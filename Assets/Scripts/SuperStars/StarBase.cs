using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarBase : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] string description;
    [SerializeField] Sprite icon;
    [SerializeField] float price;
    [SerializeField] bool isSellable;
    [SerializeField] float duration;

    public float Price => price;

    public float Duration => duration;

    public bool IsSellable => isSellable;

    public string Name => name;

    public string Description => description;

    public Sprite Icon => icon;

    public virtual bool CallingStarMarket(Marketplace marketplace)
    {
        return false;
    }

    public virtual bool CallingStarProd(Produce production)
    {
        return false;
    }

    public virtual bool IsReusable => true;

    public virtual bool CanUse => true;

    public virtual bool IsMarket => true;
}
