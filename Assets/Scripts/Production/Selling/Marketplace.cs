using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Marketplace : MonoBehaviour
{
    
    [SerializeField] Text defText;


    Rating rating;
    WorkerParty workerParty;
    List<Worker> workers;
    float rate;
    List<int> ranMinutes = new List<int>();
    int actualMinute;
    int custCont = 0;
    int salesCont = 0;
    int defsCont = 0;


    public float totalDefectives = 0;

    Inventory inventory;
    public static Action instantiateCust;

    public bool tv { get; set; } = false;
    public bool radio { get; set; } = false;
    public bool socialMedia { get; set; } = false;
    public bool salesStar { get; set; } = false;
    public bool qualityStar { get; set; } = false;

    public List<int> customers { get; set; }
    public List<int> sales { get; set; }
    public List<int> defs { get; set; }

    float tvAdDuration = 0f;
    float radioAdDuration = 0f;
    float socialAdDuration = 0f;

    float saleStarDuration = 0f;
    float qualityStarDuration = 0f;

    bool eventActive = false;
    float eventDuration = 0f;
    EventBase activeEvent;

    private void Awake()
    {
        rating = FindObjectOfType<PlayerController>().GetComponent<Rating>();
        workerParty = WorkerParty.GetWorkerParty();
        workers = workerParty.Workers;
        customers = new List<int>();
        sales = new List<int>();
        defs = new List<int>();
    }
    // Start is called before the first frame update
    private void Start()
    {
        inventory = Inventory.GetInventory();
        rate = rating.GetRating();
    }
    private void OnEnable()
    {
        TimeController.OnMinuteChanged += CheckTime;
        TimeController.OnHourChanged += StatsPerHour;
        Event.OnMarketEvent += triggerEvent;
    }

    private void OnDisable()
    {
        TimeController.OnMinuteChanged -= CheckTime;
        TimeController.OnHourChanged -= StatsPerHour;
        Event.OnMarketEvent -= triggerEvent;
    }

    private void Update()
    {
        rate = rating.GetRating();
        CalculateDefectives();
        defText.text = "Defectives: " + totalDefectives + "%";
    }

    private void CheckTime()
    {
        if (TimeController.Hour >= 8 && TimeController.Hour < 17)
        {
            if (eventDuration == 0)
            {
                eventActive = false;
            }
            if (eventDuration > 0)
            {
                eventActive = true;
                eventDuration -= 1;
            }
            if (tvAdDuration == 0)
            {
                tv = false;
            }
            if (tvAdDuration > 0)
            {
                tv = true;
                tvAdDuration -= 1;
            }

            if (radioAdDuration == 0)
            {
                radio = false;
            }
            if (radioAdDuration > 0)
            {
                radio = true;
                radioAdDuration -= 1;
            }

            if (socialAdDuration == 0)
            {
                socialMedia = false;
            }
            if (socialAdDuration > 0)
            {
                socialMedia = true;
                socialAdDuration -= 1;
            }

            if (saleStarDuration == 0)
            {
                salesStar = false;
            }
            if (saleStarDuration > 0)
            {
                salesStar = true;
                saleStarDuration -= 1;
            }

            if (qualityStarDuration == 0)
            {
                qualityStar = false;
            }
            if (qualityStarDuration > 0)
            {
                qualityStar = true;
                qualityStarDuration -= 1;
            }


            if (TimeController.Minute == 0)
            {
                //generate random
                ranMinutes = Sells(rate);

            }
            else if (TimeController.Minute % 10 == 0)
            {
                //generate random
                ranMinutes = Sells(rate);
                actualMinute = TimeController.Minute;

            }
            foreach (int plusMinutes in ranMinutes)
            {
                if (actualMinute + plusMinutes == 60)
                {
                    if (TimeController.Minute == 0)
                    {
                        StartCoroutine(SellProducts());
                    }
                }
                else
                {
                    if (TimeController.Minute == actualMinute + plusMinutes)
                    {
                        StartCoroutine(SellProducts());
                    }
                }
            }
        }
        else if (TimeController.Hour == 17)
        {
            TvAd(0f);

            RadioAd(0f);

            SocialMediaAd(0f);

            HireSalesStar(0f);

            HireQualityStar(0f);
        }
    }

    private List<int> Sells(float rateToUse)
    {
        if (salesStar)
            rateToUse = rateToUse * 1.5f;
        int minute;
        List<int> minutesToSell = new List<int>();
        int rating = (int)Math.Floor(rateToUse);
        for (int i = 0; i < rating; i++)
        {
            do
            {
                minute = RandomNumber(1,11);
            } while (minutesToSell.Contains(minute));
            minutesToSell.Add(minute);
        }
        return minutesToSell;
    }

    public void triggerEvent(EventBase possibleEvent)
    {
        eventActive = true;
        eventDuration = possibleEvent.AffectTime;
        activeEvent = possibleEvent;
        Debug.Log($"Market Event {possibleEvent.Name}");
    }

    private int RandomNumber(int minLimit, int maxLimit)
    {
        return UnityEngine.Random.Range(minLimit, maxLimit);
    }

    private bool ComeIn()
    {

        float tvFactor = 0;
        float radioFactor = 0;
        float socialFactor = 0;
        if (tv)
        {
            tvFactor = 100;
        }
        if (radio)
        {
            radioFactor = 100;
        }
        if (socialMedia)
        {
            socialFactor = 100;
        }
        float factor = rate * 20+tvFactor+radioFactor+socialFactor;
        if (eventActive && activeEvent.Sales > 0)
        {
            factor = activeEvent.Sales;
        }
        Debug.Log($"{factor}");
        if (UnityEngine.Random.Range(0, 255) <= factor)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator SellProducts()
    {
        //Sell
        
        if (ComeIn())
        {
            var product = inventory.GetItem(RandomNumber(0, inventory.GetProductCount(1)), 1);
            int cant = RandomNumber(1, 10);
            instantiateCust?.Invoke();
            yield return SellItem(product, cant);
            custCont++;
            //Debug.Log($"Customer {custCont} come in, purchased {cant} of {product.Name}");

        }
            
    }

    IEnumerator SellItem(ItemBase item, int countToSell)
    {

        if (item != null)
        {
            if (!item.IsSellable)
            {
                Debug.Log("Testing no sellable");
                yield break;
            }
            else
            {
                float sellingPrice = Mathf.Round(item.Price / 2);

                if (eventActive && activeEvent.Price > 0)
                {
                    sellingPrice = Mathf.Round(item.Price / 2)/activeEvent.Price;
                }

                int itemCount = inventory.GetItemCount(item);
                if (itemCount >= 1 && itemCount >= countToSell)
                {
                    sellingPrice = sellingPrice * countToSell;

                    inventory.RemoveItem(item, countToSell);
                    Money.i.AddMoney(sellingPrice);
                    if (SoldDefective(countToSell))
                    {
                        rating.AddDefective();
                        Debug.Log($"negative: {rating.Negative}");
                        defsCont++;
                    }
                    else
                    {
                        rating.AddPositive();
                        Debug.Log($"positive: {rating.Positive}");
                    }
                    salesCont++;

                }
                else
                {
                    Debug.Log("DSAT, 80% chance to bad review");
                    rating.AddNegative();
                    Debug.Log($"negative: {rating.Negative}");
                }
                Debug.Log($"Rating: {rating.GetRating()}");

            }
        }
        else
        {
            Debug.Log("NO PRODUCT - DSAT, 80% chance to bad review");
            rating.AddNegative();
            Debug.Log($"negative: {rating.Negative}");
        }

    }

    public void TvAd(float duration)
    {
        tvAdDuration = duration * 60f;
        tv = true;
    }

    public void RadioAd(float duration)
    {
        radioAdDuration = duration * 60f;
        radio = true;
    }

    public void SocialMediaAd(float duration)
    {
        socialAdDuration = duration * 60f;
        socialMedia = true;
    }

    public void HireSalesStar(float duration)
    {
        saleStarDuration = duration * 60f;
        salesStar = true;
    }

    public void HireQualityStar(float duration)
    {
        qualityStarDuration = duration * 60f;
        qualityStar = true;
    }

    public float CalculateDefectives()
    {
        float accuracy = 0;
        for (int i = 0; i < workers.Count; i++)
        {
            if (workers[i].Faults == null)
                continue;
            for (int j = 0; j < workers[i].Faults.Count; j++)
            {
                Option defect = workers[i].Faults[j];
                float tempAccuracy = defect.Acuraccy / 100;
                
                if (tempAccuracy > 2)
                    tempAccuracy -= 2;
                if (tempAccuracy > 1)
                    tempAccuracy -= 1;
                accuracy += tempAccuracy;
            }
            
        }
        if (qualityStar)
            accuracy = accuracy / 2;
        if (eventActive && activeEvent.Defectives > 0)
        {
            accuracy = activeEvent.Defectives;
        }
        totalDefectives = accuracy;
        return accuracy;
    }

    public float DefectiveItems(int countToSell)
    {
        
        float defFactor = CalculateDefectives();
        
        float defectives = (float)Math.Floor(countToSell * defFactor);
        return (defectives * 10) / countToSell;

    }

    public bool SoldDefective(int countToSell)
    {
        float defectivePercent = DefectiveItems(countToSell);

        if (RandomNumber(1, 101) <= defectivePercent)
        {
            Debug.Log("sold defective");
            return true;
            
        }
        Debug.Log("not defective");
        return false;
        
    }

    private void StatsPerHour()
    {
        
        customers.Add(custCont);
        sales.Add(salesCont);
        defs.Add(defsCont);
        custCont = 0;
        salesCont = 0;
        defsCont = 0;
        Debug.Log(custCont);
    }
}
