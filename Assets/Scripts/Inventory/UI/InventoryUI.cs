using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum InventoryUIState { ItemSelection, Busy, PartySelection }

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;
    [SerializeField] Color highlatedColor;
    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;
    [SerializeField] Text categoryText;
    [SerializeField] PartyScreen partyScreen;

    List<ItemSlotUI> slotUIList;
    int selectedItem = 0;
    int selectedCategory = 0;

    Inventory inventory;
    RectTransform itemListRect;
    Marketplace marketplace;

    Action<ItemBase> onItemUsed;
    InventoryUIState state;
    const int itemsInViewPort = 8;
    private void Awake()
    {
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
        marketplace = FindObjectOfType<GameController>().GetComponent<Marketplace>();
    }

    private void Start()
    {
        UpdateItemList();
        inventory.OnUpdated += UpdateItemList;
    }

    void UpdateItemList()
    {
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        slotUIList = new List<ItemSlotUI>();
        foreach (var itemSlot in inventory.GetSlotsByCategory(selectedCategory))
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetData(itemSlot);

            slotUIList.Add(slotUIObj);

        }

        UpdateItemSelection();

    }
    public void HandleUpdate(Action onBack, Action<ItemBase> onItemUsed = null)
    {
        this.onItemUsed = onItemUsed;
        if (state == InventoryUIState.ItemSelection)
        {
            int prevSelection = selectedItem;
            int prevCategory = selectedCategory;

            if (Input.GetKeyDown(KeyCode.S))
                ++selectedItem;
            else if (Input.GetKeyDown(KeyCode.W))
                --selectedItem;
            else if (Input.GetKeyDown(KeyCode.D))
                ++selectedCategory;
            else if (Input.GetKeyDown(KeyCode.A))
                --selectedCategory;

            if (selectedCategory > Inventory.ItemCategories.Count - 1)
                selectedCategory = 0;
            else if (selectedCategory < 0)
                selectedCategory = Inventory.ItemCategories.Count - 1;

            selectedItem = Mathf.Clamp(selectedItem, 0, inventory.GetSlotsByCategory(selectedCategory).Count - 1);
            selectedCategory = Mathf.Clamp(selectedCategory, 0, Inventory.ItemCategories.Count - 1);

            if (prevCategory != selectedCategory)
            {
                selectedItem = 0;
                categoryText.text = Inventory.ItemCategories[selectedCategory];
                UpdateItemList();
            }
            else if (prevSelection != selectedItem)
            {
                UpdateItemSelection();
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                StartCoroutine(ItemSelected());
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                onBack?.Invoke();
            }
        }
        else if (state == InventoryUIState.PartySelection)
        {
            Action onSelected = () =>
            {
                StartCoroutine(UseItem());
            };

            Action onBackPartyScreen = () =>
            {
                ClosePartyScreen();
            };

            partyScreen.HandleUpdate(onBackPartyScreen, onSelected);
        }
    }

    void UpdateItemSelection()
    {
        var slots = inventory.GetSlotsByCategory(selectedCategory);

        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItem)
            {
                slotUIList[i].NameText.color = highlatedColor;
            }
            else
            {
                slotUIList[i].NameText.color = Color.black;
            }
                
        }

        selectedItem = Mathf.Clamp(selectedItem, 0, slots.Count - 1);

        if (slots.Count > 0)
        {
            var item = slots[selectedItem].Item;
            itemIcon.sprite = item.Icon;
            itemDescription.text = item.Description;
        }
        

        HandleScrolling();
    }

    void HandleScrolling()
    {
        if (selectedItem >= 0)
        {
            float scrollPos = Mathf.Clamp(selectedItem - (itemsInViewPort / 2), 0, selectedItem) * slotUIList[0].Height;
            itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);
        }
        

    }

    IEnumerator ItemSelected()
    {
        state = InventoryUIState.Busy;

        var item = inventory.GetItem(selectedItem, selectedCategory);

        if (item != null)
        {
            if (GameController.Instance.State == GameState.Shop)
            {
                onItemUsed?.Invoke(item);
                state = InventoryUIState.ItemSelection;
                yield break;
            }
            else
            {
                if (!item.CanUse)
                {
                    yield return DialogManager.Instance.ShowDialogText($"This item cannot be used");
                    state = InventoryUIState.ItemSelection;
                    yield break;
                }
            }

            if (item is MarketingItem)
            {
                int selectedChoice = 0;
                yield return DialogManager.Instance.ShowDialogText($"Do you want to activate {item.Name}?",
                    waitForInput: false,
                    choices: new List<string>() { "Yes", "No" },
                    onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);
                if (selectedChoice == 0)
                {
                    var usedItem = inventory.UseMarketItem(selectedItem, marketplace, selectedCategory);

                    if (usedItem != null)
                    {
                        yield return DialogManager.Instance.ShowDialogText($"Advertisement {item.Name} is active for {item.Duration} hours");
                        onItemUsed?.Invoke(item);
                    }
                    else
                    {
                        yield return DialogManager.Instance.ShowDialogText($"Advertisement already active");
                    }
                }
                state = InventoryUIState.ItemSelection;

            }
            else
            {
                OpenPartyScreen();
            }

        }
        else
        {
            state = InventoryUIState.ItemSelection;
            yield break;
        }

    }

    void OpenPartyScreen()
    {
        state = InventoryUIState.PartySelection;
        partyScreen.gameObject.SetActive(true);
    }

    void ClosePartyScreen()
    {
        state = InventoryUIState.ItemSelection;

        partyScreen.gameObject.SetActive(false);
    }

    IEnumerator UseItem()
    {
        state = InventoryUIState.Busy;

        var item = inventory.GetItem(selectedItem, selectedCategory);

        var usedItem = inventory.UseItem(selectedItem, partyScreen.SelectedMember, selectedCategory);
        if (usedItem != null)
        {
            if (usedItem is BoosterItem)
                yield return DialogManager.Instance.ShowDialogText($"The player used {usedItem.Name}");
            
                

            onItemUsed?.Invoke(usedItem);
        }
        else
        {
            if (selectedCategory == (int)ItemCategory.Items)
                yield return DialogManager.Instance.ShowDialogText($"It won't have any affect!");
        }

        ClosePartyScreen();
    }


    IEnumerator SellItems()
    {
        state = InventoryUIState.Busy;
        var item = inventory.GetItem(selectedItem, selectedCategory);

        if (GameController.Instance.State == GameState.Shop)
        {
            onItemUsed?.Invoke(item);
            state = InventoryUIState.ItemSelection;
            yield break;
        }
    }
}

