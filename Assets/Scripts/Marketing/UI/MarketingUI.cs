using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MarketingUIState { AdSelection, Busy }
public class MarketingUI : MonoBehaviour
{

    [SerializeField] GameObject adsList;
    [SerializeField] MarketingItemUI adSlotUI;
    [SerializeField] Image adIcon;
    [SerializeField] Text adDescription;
    [SerializeField] MoneyUI moneyUI;

    List<MarketingItemUI> slotUIList;

       
    int selectedAd = 0;

    Marketing marketingItems;
    RectTransform adListRect;
    Marketplace marketplace;

    Action<ItemBase> onAdCalled;
    MarketingUIState state;
    const int itemsInViewPort = 8;
    private void Awake()
    {
        marketingItems = Marketing.GetMarketing();
        adListRect = adsList.GetComponent<RectTransform>();
        marketplace = FindObjectOfType<GameController>().GetComponent<Marketplace>();
    }

    public void Show()
    {
        moneyUI.Show();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        moneyUI.Close();
        gameObject.SetActive(false);
    }

    private void Start()
    {
        UpdateAdsList();
        marketingItems.OnUpdated += UpdateAdsList;
    }

    void UpdateAdsList()
    {
        foreach (Transform child in adsList.transform)
        {
            Destroy(child.gameObject);
        }


        slotUIList = new List<MarketingItemUI>();
        foreach (var marketingSlot in marketingItems.GetSlots())
        {
            var slotUIObj = Instantiate(adSlotUI, adsList.transform);
            slotUIObj.SetData(marketingSlot);

            slotUIList.Add(slotUIObj);

        }

        UpdateMarketingSelection();

    }
    public void HandleUpdate(Action onBack, Action<ItemBase> onAdCalled = null)
    {
        this.onAdCalled = onAdCalled;
        if (state == MarketingUIState.AdSelection)
        {
            int prevSelection = selectedAd;

            if (Input.GetKeyDown(KeyCode.S))
                ++selectedAd;
            else if (Input.GetKeyDown(KeyCode.W))
                --selectedAd;

            selectedAd = Mathf.Clamp(selectedAd, 0, marketingItems.GetSlots().Count - 1);


            if (prevSelection != selectedAd)
            {
                UpdateMarketingSelection();
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                StartCoroutine(AdSelected());
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                onBack?.Invoke();
            }

        }
    }

    void UpdateMarketingSelection()
    {
        var slots = marketingItems.GetSlots();

        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedAd)
            {
                slotUIList[i].NameText.color = GlobalSettings.i.HighlightedColor;
            }
            else
            {
                slotUIList[i].NameText.color = Color.black;
            }

        }

        selectedAd = Mathf.Clamp(selectedAd, 0, slots.Count - 1);

        if (slots.Count > 0)
        {
            var advertisement = slots[selectedAd].Advertisement;
            adIcon.sprite = advertisement.Icon;
            adDescription.text = advertisement.Description;
        }

        HandleScrolling();

    }

    void HandleScrolling()
    {
        if (selectedAd >= 0)
        {
            float scrollPos = Mathf.Clamp(selectedAd - (itemsInViewPort / 2), 0, selectedAd) * slotUIList[0].Height;
            adListRect.localPosition = new Vector2(adListRect.localPosition.x, scrollPos);
        }


    }

    IEnumerator AdSelected()
    {
        state = MarketingUIState.Busy;

        var adv = marketingItems.GetMarketingItem(selectedAd);

        if (!adv.CanUse)
        {
            yield return DialogManager.Instance.ShowDialogText($"Ad cannot be used");
            state = MarketingUIState.AdSelection;
            yield break;
        }

        int selectedChoice = 0;
        yield return DialogManager.Instance.ShowDialogText($"Do you want to use {adv.Name} for {adv.Price}?",
            waitForInput: false,
            choices: new List<string>() { "Yes", "No" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);
        if (selectedChoice == 0)
        {

            if (Money.i.HasMoney(adv.Price))
            {

                ItemBase useAdv;

                useAdv = marketingItems.CallMarketAd(selectedAd, marketplace);

                if (useAdv != null)
                {
                    Money.i.TakeMoney(adv.Price);
                    yield return DialogManager.Instance.ShowDialogText($"{adv.Name} is active");
                    onAdCalled?.Invoke(adv);
                }
                else
                {
                    yield return DialogManager.Instance.ShowDialogText($"Ad already active");
                }
            }
            else
            {
                yield return DialogManager.Instance.ShowDialogText("Not enough money for that!");
            }

        }
        state = MarketingUIState.AdSelection;


    }

}
