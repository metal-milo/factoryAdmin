using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour
{
    [SerializeField] float moneyQuantity;
    float moneySpent = 0;
    float moneyFrmSales = 0;

    public float MoneyQuantity => moneyQuantity;
    public float MoneySpent => moneySpent;
    public float MoneyFrmSales => moneyFrmSales;

    public event Action OnMoneyChanged;

    public static Money i { get; private set; }
    private void Awake()
    {
        i = this;
    }

    public void AddMoney(float amount)
    {
        moneyQuantity += amount;
        moneyFrmSales += amount;
        OnMoneyChanged?.Invoke();
    }

    public void TakeMoney(float amount)
    {
        moneyQuantity -= amount;
        moneySpent += amount;
        OnMoneyChanged?.Invoke();
        
    }

    public bool HasMoney(float amount)
    {
        return amount <= moneyQuantity;
    }

    public void RestartMoney()
    {
        moneySpent = 0;
        moneyFrmSales = 0;
    }
}
