using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemCategory { RawMaterial, Product, Items, Marketing }

public class Inventory : MonoBehaviour
{
    
    [SerializeField] List<ItemSlot> rawMaterialSlots;
    [SerializeField] List<ItemSlot> completeProductSlots;
    [SerializeField] List<ItemSlot> items;
    [SerializeField] List<ItemSlot> marketing;

    List<List<ItemSlot>> allSlots;

    public event Action OnUpdated;

    private void Awake()
    {
        allSlots = new List<List<ItemSlot>>() { rawMaterialSlots, completeProductSlots, items, marketing };
    }

    public static List<string> ItemCategories { get; set; } = new List<string>()
    {
        "MATERIALS", "PRODUCTS", "ITEMS", "MARKETING"
    };

    public List<ItemSlot> GetSlotsByCategory(int categoryIndex)
    {
        return allSlots[categoryIndex];
    }



    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }

    public void AddItem(ItemBase item, int count = 1)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);

        var itemSlot = currentSlots.FirstOrDefault(slot => slot.Item == item);
        if (itemSlot != null)
        {
            itemSlot.Count += count;
        }
        else
        {
            currentSlots.Add(new ItemSlot()
            {
                Item = item,
                Count = count
            });
        }

        OnUpdated?.Invoke();
    }

    public void AddProduce(ItemBase item, int count = 1)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);

        var itemSlot = currentSlots.FirstOrDefault(slot => slot.Item == item);
        if (itemSlot != null)
        {
            itemSlot.Count += count;
        }
        else
        {
            currentSlots.Add(new ItemSlot()
            {
                Item = item,
                Count = count
            });
        }

    }

    public ItemBase GetItem(int itemIndex, int categoryIndex)
    {
        var currenSlots = GetSlotsByCategory(categoryIndex);
        if (currenSlots.Count > 0)
            return currenSlots[itemIndex].Item;
        Debug.Log("no hay");
        return null;
    }

    public bool HasItem(ItemBase item)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);

        return currentSlots.Exists(slot => slot.Item == item);
    }

    public int GetProductCount(int categoryIndex)
    {
        var currenSlots = GetSlotsByCategory(categoryIndex);
        return currenSlots.Count;
    }

    public int GetItemCount(ItemBase item)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);

        var itemSlot = currentSlots.FirstOrDefault(slot => slot.Item == item);

        if (itemSlot != null)
        {
            return itemSlot.Count;
        }
        else
        {
            return 0;
        }
    }

    public void RemoveItem(ItemBase item, int countToRemove = 1)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);

        var itemSlot = currentSlots.First(slot => slot.Item == item);
        if (itemSlot.Count >= countToRemove)
            itemSlot.Count -= countToRemove;
        if (itemSlot.Count == 0)
            currentSlots.Remove(itemSlot);

        OnUpdated?.Invoke();
    }

    ItemCategory GetCategoryFromItem(ItemBase item)
    {
        if (item is RawMaterialItem)
            return ItemCategory.RawMaterial;
        else if (item is CompleteProductItem)
            return ItemCategory.Product;
        else if (item is MarketingItem)
            return ItemCategory.Marketing;
        else
            return ItemCategory.Items;
    }

    public ItemBase UseItem(int itemIndex, Worker selectedWorker, int selectedCategory)
    {
        var item = GetItem(itemIndex, selectedCategory);
        bool itemUsed = item.UseOnWorker(selectedWorker);
        if (itemUsed)
        {
            if (!item.IsReusable)
                RemoveItem(item);

            return item;
        }

        return null;
    }

    public ItemBase UseMarketItem(int itemIndex, Marketplace marketplace, int selectedCategory)
    {
        var item = GetItem(itemIndex, selectedCategory);
        bool itemUsed = item.UseOnMarket(marketplace);
        if (itemUsed)
        {
            if (!item.IsReusable)
                RemoveItem(item);

            return item;
        }

        return null;
    }

    public float GetWarehouseCost()
    {
        float warehouseCosts = 0f;
        foreach (var product in completeProductSlots){
            warehouseCosts += (product.Item.Price / 4) * product.Count;
        }
        return warehouseCosts;
    }

}

[Serializable]
public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int count;

    public ItemBase Item
    {
        get => item;
        set => item = value;
    }
    public int Count
    {
        get => count;
        set => count = value;
    }
}
