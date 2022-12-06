using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] Text moneyText;

    private void Start()
    {
        Money.i.OnMoneyChanged += SetMoneyTxt;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        SetMoneyTxt();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    void SetMoneyTxt()
    {
        moneyText.text = "$ " + Money.i.MoneyQuantity;
    }
}
