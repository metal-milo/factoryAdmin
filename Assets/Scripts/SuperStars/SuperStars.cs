using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperStars : MonoBehaviour
{
    
    [SerializeField] List<StarSlot> slots;

    public event Action OnUpdated;

    public List<StarSlot> GetSlots()
    {
        return slots;
    }


    public static SuperStars GetStars()
    {
        return FindObjectOfType<PlayerController>().GetComponent<SuperStars>();
    }

    public StarBase GetStar(int starIndex)
    {
        var currenSlots = GetSlots();
        return currenSlots[starIndex].Star;
    }

    public StarBase CallSuperStarMarket(int starIndex, Marketplace marketplace)
    {
        var star = GetStar(starIndex);
        bool starCall = star.CallingStarMarket(marketplace);
        if (starCall)
        {
            return star;
        }

        return null;
    }

    public StarBase CallSuperStarProd(int starIndex, Produce production)
    {
        var star = GetStar(starIndex);
        bool starCall = star.CallingStarProd(production);
        if (starCall)
        {
            return star;
        }

        return null;
    }

    public bool IsMarketStar(int starIndex)
    {
        var star = GetStar(starIndex);
        if (star.IsMarket)
            return true;

        return false;
    }

}

[Serializable]
public class StarSlot
{
    [SerializeField] StarBase star;
    [SerializeField] int price;

    public StarBase Star
    {
        get => star;
        set => star = value;
    }
    public int Price
    {
        get => price;
        set => price = value;
    }
}