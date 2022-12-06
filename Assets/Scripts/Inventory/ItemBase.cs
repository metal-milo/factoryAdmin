using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] string description;
    [SerializeField] Sprite icon;
    [SerializeField] float price;
    [SerializeField] bool isSellable;
    [SerializeField] float duration;
    [SerializeField] bool isRaw;
    [SerializeField] bool isProduct;

    public float Price => price;

    public float Duration => duration;

    public bool IsSellable => isSellable;

    public string Name => name;

    public string Description => description;

    public Sprite Icon => icon;

    public virtual bool UseOnWorker(Worker worker)
    {
        return false;
    }

    public virtual bool UseOnMarket(Marketplace marketplace)
    {
        return false;
    }

    public virtual bool IsReusable  => false;

    public virtual bool CanUse => true;

}
