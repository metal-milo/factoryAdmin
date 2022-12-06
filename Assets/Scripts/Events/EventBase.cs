using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Event", menuName = "Event/Create new event")]
public class EventBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite sprite;

    [SerializeField] EventType type;

    [SerializeField] int affectTime;
    [SerializeField] int production;
    [SerializeField] int price;
    [SerializeField] int defectives;
    [SerializeField] int sales;

    [SerializeField] int happenRate = 255;
    [SerializeField] bool completeDay;

    public string Name
    {
        get { return name; }
    }

    public string Description
    {
        get { return description; }
    }

    public Sprite Sprite
    {
        get { return sprite; }
    }

 
    public EventType Type
    {
        get { return type; }
    }


    public int AffectTime
    {
        get { return affectTime; }
    }

    public int Production
    {
        get { return production; }
    }

    public int Price
    {
        get { return price; }
    }

    public int Defectives
    {
        get { return defectives; }
    }

    public int Sales
    {
        get { return sales; }
    }

    public bool CompleteDay
    {
        get { return completeDay; }
    }
    public int HappenRate => happenRate;

}


public enum EventType
{
    prod,
    market,
    time
}




