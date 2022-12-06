using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RateBase : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] string description;
    [SerializeField] Sprite icon;
    [SerializeField] bool isPositive;

    public string Name => name;

    public string Description => description;

    public Sprite Icon => icon;

    public bool IsPositive => isPositive;
}
