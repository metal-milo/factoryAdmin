using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;

    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;


    int selectedItem;

    List<ItemBase> rawMaterials;
    Action<ItemBase> onItemSelected;
    Action onBack;

    List<ItemSlotUI> slotUIList;

    const int itemsInViewport = 8;

    RectTransform itemListRect;
    private void Awake()
    {
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    public void Show(List<ItemBase> rawMaterials, Action<ItemBase> onItemSelected,
        Action onBack)
    {
        this.rawMaterials = rawMaterials;
        this.onItemSelected = onItemSelected;
        this.onBack = onBack;

        gameObject.SetActive(true);
        UpdateItemList();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void HandleUpdate()
    {
        var prevSelection = selectedItem;

        if (Input.GetKeyDown(KeyCode.S))
            ++selectedItem;
        else if (Input.GetKeyDown(KeyCode.W))
            --selectedItem;

        selectedItem = Mathf.Clamp(selectedItem, 0, rawMaterials.Count - 1);

        if (selectedItem != prevSelection)
            UpdateItemSelection();

        if (Input.GetKeyDown(KeyCode.Z))
            onItemSelected?.Invoke(rawMaterials[selectedItem]);
        else if (Input.GetKeyDown(KeyCode.X))
            onBack?.Invoke();
    }

    void UpdateItemList()
    {
        // Clear all the existing items
        foreach (Transform child in itemList.transform)
            Destroy(child.gameObject);

        slotUIList = new List<ItemSlotUI>();
        foreach (var item in rawMaterials)
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetNameAndPrice(item);

            slotUIList.Add(slotUIObj);
        }

        UpdateItemSelection();
    }

    void UpdateItemSelection()
    {

        selectedItem = Mathf.Clamp(selectedItem, 0, rawMaterials.Count - 1);

        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItem)
                slotUIList[i].NameText.color = GlobalSettings.i.HighlightedColor;
            else
                slotUIList[i].NameText.color = Color.black;
        }

        if (rawMaterials.Count > 0)
        {
            var item = rawMaterials[selectedItem];
            itemIcon.sprite = item.Icon;
            itemDescription.text = item.Description;
        }

        HandleScrolling();
    }

    void HandleScrolling()
    {
        if (slotUIList.Count <= itemsInViewport) return;

        float scrollPos = Mathf.Clamp(selectedItem - itemsInViewport / 2, 0, selectedItem) * slotUIList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);
        
    }
}