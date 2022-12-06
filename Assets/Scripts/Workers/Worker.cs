using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[System.Serializable]
public class Worker
{
    [SerializeField] WorkerBase _base;
    [SerializeField] int level;
    List<Option> privFaults;

    public event Action OnStatusChanged;
    public float hourlyProduction;

    public float defectivesPercent;

    public int MaxSpeed { get; private set; }
    public int speed { get; set; }
    public Mood Status { get; private set; }
    public List<Option> Faults { get; set; }
    public Queue<string> StatusChanges { get; private set; }

    public int raise = 0;

    public WorkerBase Base
    {
        get
        {
            return _base;
        }
    }
    public int Level
    {
        get
        {
            return level;
        }
    }

    public List<Option> PrivFaults
    {
        get
        {
            return privFaults;
        }
    }

    public Worker(WorkerBase wBase, int wLevel)
    {
        _base = wBase;
        level = wLevel;
        Init();
    }


    public void Init()
    {
       
        Faults = new List<Option>();

        float randomNumber = UnityEngine.Random.Range(0, 5);

        Debug.Log($"will have {randomNumber} faults");
        StatusChanges = new Queue<string>();
        if (randomNumber != 0)
        {

            Faults = generateFaults(randomNumber);
            CalculateDefectives();
        }
        CalculateStats();
        
    }

    void CalculateStats()
    {
        
        int oldMaxSpeed = _base.MaxSpeed;
        MaxSpeed = Mathf.FloorToInt((Base.Health * Level) / 100f) + 10 + Level;

        if (oldMaxSpeed != 0)
            speed += oldMaxSpeed - MaxSpeed;
        Debug.Log($"{_base.name} has {speed} speed");



        hourlyProduction = RoundToProd(speed);
        Debug.Log($"{_base.name} has {hourlyProduction} production");
    }

    void CalculateDefectives()
    {
        float accuracy = 0;
        
        for (int j = 0; j < Faults.Count; j++)
        {
            Option defect = Faults[j];
            float tempAccuracy = defect.Acuraccy / 100;
            

            if (tempAccuracy > 2)
                tempAccuracy -= 2;
            if (tempAccuracy > 1)
                tempAccuracy -= 1;
            accuracy += tempAccuracy;
        }
        
        
        defectivesPercent = accuracy;

    }

    public float RoundToProd(int speed)
    {
        float y = speed;
        float x = y / 30;
        double z = Math.Round(x);
        return (float)z/2f;
    }

    public int Health
    {
        get { return Mathf.FloorToInt((Base.Health * Level) / 100f) + 10; }
    }

    public int Salary
    {
        get { return Mathf.FloorToInt(((Base.Salary + raise) * Level) / 100f) + 5; }
    }

    public void Motivate()
    {
        Status = MoodDB.Moods[MoodId.happy];
        OnStatusChanged?.Invoke();
    }

   
    public void GiveRaise(int raise)
    {
        Status = MoodDB.Moods[MoodId.great];
        this.raise = raise;
        OnStatusChanged?.Invoke();
    }


    public void SetStatus(MoodId moodId)
    {
        Status = MoodDB.Moods[moodId];
        StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");
    }

    public void OnBeforeShift()
    {
        Status?.OnBeforeShift?.Invoke(this);
        hourlyProduction = RoundToProd(speed);
    }

    public void IncreaseSpeed(int amount)
    {
        speed = Mathf.Clamp(speed + amount, 0, MaxSpeed);
    }

    public void DecreaseSpeed(int amount)
    {
        speed = Mathf.Clamp(speed - amount, 0, MaxSpeed);
    }

    public List<Option> generateFaults(float random)
    {
        List<Option> returnFaults = new List<Option>();
        List<int> selectedFaults = FaultRandomizer(random);
        foreach (var fault in Base.PossibleFaults)
        {
            foreach (int faultIn in selectedFaults)
            {
                if (fault.Randomized == faultIn)
                {
                    Debug.Log(fault.Base);
                    returnFaults.Add(new Option(fault.Base));
                }
            }
        }
        return returnFaults;
    }

    private List<int> FaultRandomizer(float randomToUse)
    {
        int fault;
        List<int> cantFaults = new List<int>();
        for (int i = 0; i < randomToUse; i++)
        {
            do
            {
                fault = UnityEngine.Random.Range(1, 5);
            } while (cantFaults.Contains(fault));
            cantFaults.Add(fault);
        }
        return cantFaults;
    }

    

    public Worker(WorkerSaveData saveData)
    {
        _base = WorkerDB.GetObjectByName(saveData.name);
        speed = saveData.speed;
        level = saveData.level;

        if (saveData.statusId != null)
            Status = MoodDB.Moods[saveData.statusId.Value];
        else
            Status = null;

        Faults = saveData.faults.Select(s => new Option(s)).ToList();

        CalculateStats();
        StatusChanges = new Queue<string>();
    }
    
    public WorkerSaveData GetSaveData()
    {
        var saveData = new WorkerSaveData()
        {
            name = Base.name,
            speed = speed,
            level = Level,
            statusId = Status?.Id,
            faults = Faults.Select(m => m.GetSaveData()).ToList()
        };

        return saveData;
    }
    
}


[System.Serializable]
public class WorkerSaveData
{
    public string name;
    public int speed;
    public int level;
    public int exp;
    public MoodId? statusId;
    public List<OptionSaveData> faults;
}