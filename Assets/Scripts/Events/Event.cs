using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event : MonoBehaviour
{

    public static List<EventBase> productives = new List<EventBase>();
    public static List<EventBase> markets = new List<EventBase>();
    public static List<EventBase> time = new List<EventBase>();
    List<List<EventBase>> listOfEvents = new List<List<EventBase>>();

    List<int> minutesToEvent = new List<int>();

    public static Action<EventBase> OnProdEvent;
    public static Action<EventBase> OnMarketEvent;
    public static Action<EventBase> OnTimeEvent;

    bool isActiveProd = false;
    bool isActiveMarke = false;
    bool isActiveTime = false;

    public static Event i { get; private set; }
    private void Awake()
    {
        Dictionary<string, EventBase> objects = EventDB.objects;
        foreach (var objectEvent in objects)
        {
            if (objectEvent.Value.Type == EventType.prod)
            {
                productives.Add(objectEvent.Value);
            }
            else if (objectEvent.Value.Type == EventType.market)
            {
                markets.Add(objectEvent.Value);
            }
            else
            {
                time.Add(objectEvent.Value);
            }
        }
        listOfEvents.Add(productives);
        listOfEvents.Add(markets);
        listOfEvents.Add(time);
        i = this;
    }

    private void OnEnable()
    {
        TimeController.OnMinuteChanged += CheckTime;
    }

    private void OnDisable()
    {
        TimeController.OnMinuteChanged -= CheckTime;
    }


    public static Event GetEvents()
    {
        return FindObjectOfType<GameController>().GetComponent<Event>();
    }

    private void CheckTime()
    {
        if (TimeController.Hour >= 8 && TimeController.Hour <= 17)
        {
            if (TimeController.Minute == 1)
            {
                CreateEventMinute();
            }
            
        }
        if (minutesToEvent.Contains(TimeController.Minute))
        {
            StartCoroutine(CheckForEvents());
        }

    }

    public IEnumerator CheckForEvents()
    {

        int eventType = RandomNumber(0, listOfEvents.Count);
        List<EventBase> typeOfEvent = listOfEvents[eventType];
        int happenEvent = RandomNumber(0, typeOfEvent.Count);
        EventBase possibleEvent = typeOfEvent[happenEvent];

        if (RandomNumber(0, possibleEvent.HappenRate) < 5)
        {
            string dialogEvent = "";
            List<string> dialogEventList = new List<string>();
            dialogEvent = possibleEvent.Name + " is active for " + possibleEvent.AffectTime;
            dialogEventList.Add(dialogEvent);
            
            if (possibleEvent.Type == EventType.prod)
            {
                dialogEvent = "Production will be " + possibleEvent.Production;
                dialogEventList.Add(dialogEvent);
                Dialog dialog = new Dialog(dialogEventList);
                yield return DialogManager.Instance.ShowDialog(dialog);
                OnProdEvent?.Invoke(possibleEvent);
            }
            else if (possibleEvent.Type == EventType.market)
            {
                if (possibleEvent.Price > 0)
                {
                    dialogEvent = "price will be " + possibleEvent.Price;
                    dialogEventList.Add(dialogEvent);
                }
                if (possibleEvent.Defectives > 0)
                {
                    dialogEvent = "Defectives will go up to " + possibleEvent.Defectives + "%";
                    dialogEventList.Add(dialogEvent);
                }
                if (possibleEvent.Sales > 0)
                {
                    dialogEvent = "Sales will go down to " + possibleEvent.Sales + "/hour";
                    dialogEventList.Add(dialogEvent);
                }

                Dialog dialog = new Dialog(dialogEventList);
                yield return DialogManager.Instance.ShowDialog(dialog);
                OnMarketEvent?.Invoke(possibleEvent);
            }
            else if (possibleEvent.Type == EventType.time)
            {
                Dialog dialog = new Dialog(dialogEventList);
                yield return DialogManager.Instance.ShowDialog(dialog);
                OnTimeEvent?.Invoke(possibleEvent);
            }
        }

    }


    private void CreateEventMinute()
    {
        int minute;
        minutesToEvent = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            do
            {
                minute = RandomNumber(3, 60);
            } while (minutesToEvent.Contains(minute));
            Debug.Log(minute);
            minutesToEvent.Add(minute);
        }
    }

    private int RandomNumber(int minLimit, int maxLimit)
    {
        return UnityEngine.Random.Range(minLimit, maxLimit);
    }
}
