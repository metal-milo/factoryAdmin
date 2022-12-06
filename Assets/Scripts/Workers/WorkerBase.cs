using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Worker", menuName = "Worker/Create new worker")]
public class WorkerBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;

    [SerializeField] WorkerType type1;

    // Base Stats
    [SerializeField] int maxSpeed;
    [SerializeField] int health;
    [SerializeField] int salary;

    [SerializeField] List<PossibleFaults> possibleFaults;

    public string Name
    {
        get { return name; }
    }

    public string Description
    {
        get { return description; }
    }

    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }

    public WorkerType Type1
    {
        get { return type1; }
    }

    public int MaxSpeed
    {
        get { return maxSpeed; }
    }

    public int Health
    {
        get { return health; }
    }

    public int Salary
    {
        get { return salary; }
    }

    public List<PossibleFaults> PossibleFaults
    {
        get { return possibleFaults; }
    }

}

[System.Serializable]
public class PossibleFaults
{
    [SerializeField] OptionBase faultBase;
    [SerializeField] int randomized;

    public OptionBase Base
    {
        get { return faultBase; }
    }

    public int Randomized
    {
        get { return randomized; }
    }
}

public enum WorkerType
{
    None,
    Slow,
    Normal,
    Fast
}
   

