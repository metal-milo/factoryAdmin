using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishDayFader : MonoBehaviour
{
    public static FinishDayFader i { get; private set; }
    [SerializeField] Image image;
    [SerializeField] Text startMoneyTxt;
    [SerializeField] Text spentMoneyTxt;
    [SerializeField] Text warehouseTxt;
    [SerializeField] Text salesTxt;
    [SerializeField] Text salariesTxt;
    [SerializeField] Text totalTxt;
    
    private void Awake()
    {
        i = this;

    }



    public IEnumerator FadeIn(float time, int startMoney, int spentMoney, int warehouseCost, int sales, int salaries, int total)
    {
        yield return startMoneyTxt.DOFade(1f, time);
        yield return spentMoneyTxt.DOFade(1f, time);
        yield return warehouseTxt.DOFade(1f, time);
        yield return salesTxt.DOFade(1f, time);
        yield return totalTxt.DOFade(1f, time);
        yield return salariesTxt.DOFade(1f, time);
        
        startMoneyTxt.text = "Initial money: " + startMoney;
        startMoneyTxt.gameObject.SetActive(true);

        spentMoneyTxt.text = "Spent money: " + spentMoney;
        spentMoneyTxt.gameObject.SetActive(true);

        warehouseTxt.text = "Warehouse cost: " + warehouseCost;
        warehouseTxt.gameObject.SetActive(true);

        salesTxt.text = "Sales: " + sales;
        salesTxt.gameObject.SetActive(true);

        salariesTxt.text = "Salaries: " + salaries;
        salariesTxt.gameObject.SetActive(true);

        totalTxt.text = "Total: " + total;
        totalTxt.gameObject.SetActive(true);
        yield return image.DOFade(1f, time).WaitForCompletion();
    }

    public IEnumerator FadeOut(float time)
    {
        
        yield return image.DOFade(0f, time).WaitForCompletion();
        yield return startMoneyTxt.DOFade(0f, time);
        yield return spentMoneyTxt.DOFade(0f, time);
        yield return warehouseTxt.DOFade(0f, time);
        yield return salesTxt.DOFade(0f, time);
        yield return totalTxt.DOFade(0f, time);
        yield return salariesTxt.DOFade(0f, time);
        /*
        startMoneyTxt.gameObject.SetActive(false);
        spentMoneyTxt.gameObject.SetActive(false);
        salesTxt.gameObject.SetActive(false);
        totalTxt.gameObject.SetActive(false);
        salariesTxt.gameObject.SetActive(false);
        */
    }
}
