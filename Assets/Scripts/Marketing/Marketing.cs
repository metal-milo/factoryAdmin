using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marketing : MonoBehaviour
{
    [SerializeField] List<MarketItemSlot> slots;

    public event Action OnUpdated;

    public List<MarketItemSlot> GetSlots()
    {
        return slots;
    }


    public static Marketing GetMarketing()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Marketing>();
    }

    public ItemBase GetMarketingItem(int index)
    {
        var currenSlots = GetSlots();
        return currenSlots[index].Advertisement;
    }

    public ItemBase CallMarketAd(int index, Marketplace marketplace)
    {
        var adv = GetMarketingItem(index);
        bool advrCall = adv.UseOnMarket(marketplace);
        if (advrCall)
        {
            return adv;
        }

        return null;
    }

    

}

[Serializable]
public class MarketItemSlot
{
    [SerializeField] ItemBase advertisement;
    [SerializeField] int price;

    public ItemBase Advertisement
    {
        get => advertisement;
        set => advertisement = value;
    }
    public int Price
    {
        get => price;
        set => price = value;
    }
}