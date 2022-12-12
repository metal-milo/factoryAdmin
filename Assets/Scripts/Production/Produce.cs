using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Produce : MonoBehaviour
{

    public bool prodIncrease { get; set; } = false;
    float prodStarDuration = 0f;
    float hourlyProduction = 0f;

    bool eventActive = false;
    float eventDuration = 0f;
    EventBase activeEvent;

    [SerializeField] Text prodText;

    Inventory inventory;
    WorkerParty workerParty;

    List<Worker> workers;
    List<int> minutesToProduce = new List<int>();
    // Start is called before the first frame update
    private void Start()
    {
        
        
    }

    private void Awake()
    {
        inventory = Inventory.GetInventory();
        workerParty = WorkerParty.GetWorkerParty();
        workers = workerParty.Workers;
        prodText.text = "Production: " + hourlyProduction + "/h";
    }
    private void OnEnable()
    {
        TimeController.OnMinuteChanged += CheckTime;
        Event.OnProdEvent += triggerEvent;
    }

    private void OnDisable()
    {
        TimeController.OnMinuteChanged -= CheckTime;
        Event.OnProdEvent -= triggerEvent;
    }

    private void CheckTime()
    {
        if (TimeController.Hour >= 8 && TimeController.Hour <= 17)
        {
            CalculateProduction();
            prodText.text = "Production: " + hourlyProduction + "/h";
            if (prodStarDuration == 0)
            {
                prodIncrease = false;
            }
            if (eventDuration == 0)
            {
                eventActive = false;
            }
            if (prodStarDuration > 0)
            {
                prodIncrease = true;
                prodStarDuration -= 1;
            }
            if (eventDuration > 0)
            {
                eventActive = true;
                eventDuration -= 1;
            }
            /*
            if (prodIncrease)
            {
                if (TimeController.Minute % 6 == 0)
                {
                    StartCoroutine(CreateProducts());
                }
            }
            else
            {
                if (TimeController.Minute % 10 == 0)
                {
                    StartCoroutine(CreateProducts());
                }
            }
            */
            if (TimeController.Minute == 1)
            {
                CreateProduct(hourlyProduction);
            }

            if (minutesToProduce.Contains(TimeController.Minute))
            {
                StartCoroutine(CreateProducts());
            }
                
        }
    }

    private IEnumerator CreateProducts()
    {
        //Produce
        Debug.Log("Produce!");
        var rawItem = inventory.GetItem(0, 0);
        var product = inventory.GetItem(0, 1);
        if (rawItem != null && product != null)
        {
            inventory.RemoveItem(rawItem, 2);
            inventory.AddProduce(product);
        }
        else
        {
            Debug.Log("no hay materia prima");
            yield return DialogManager.Instance.ShowDialogText($"Need to buy material!");
        }
        yield return null;
    }

    public void Increase(float duration)
    {
        prodStarDuration = duration * 60f;
        prodIncrease = true;
    }

    public void triggerEvent(EventBase possibleEvent)
    {
        eventActive = true;
        eventDuration = possibleEvent.AffectTime;
        activeEvent = possibleEvent;
        Debug.Log($"Prod Event {possibleEvent.Name}");
    }

    public void CalculateProduction()
    {
        float production = 0;
        for (int i = 0; i < workers.Count; i++)
        {

            production += workers[i].hourlyProduction;

        }
        if (prodIncrease)
        {
            production = production * 1.5f;
        }
        if (eventActive)
        {
            if (activeEvent.Production == 0)
                production = 0;
            else
                production = production - activeEvent.Production;
        }

        hourlyProduction = production;
    }

    
    private void CreateProduct(float hourlyProd)
    {
        if (hourlyProd != 0)
        {
            int minute;
            minutesToProduce = new List<int>();
            int hourProd;
            if (hourlyProd < 1)
                hourProd = (int)Math.Ceiling((60 * hourlyProd));
            else
                hourProd = (int)Math.Ceiling((60 / hourlyProd));
            while (hourProd < 60)
            {
                minute = hourProd;
                minutesToProduce.Add(minute);
                hourProd += hourProd;
            }
        }
        

        
    }
    
    private int RandomNumber(int minLimit, int maxLimit)
    {
        return UnityEngine.Random.Range(minLimit, maxLimit);
    }

}
