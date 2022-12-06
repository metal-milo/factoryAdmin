using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Worker", menuName = "Worker/Create new option")]
public class OptionBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] OptionType type;
    [SerializeField] int accuracy;

    public string Name
    {
        get { return name; }
    }

    public string Description
    {
        get { return description; }
    }

    public OptionType Type
    {
        get { return type; }
    }

    public int Accuracy
    {
        get { return accuracy; }
    }
}

public enum OptionType
{
    None,
    Low,
    Normal,
    High,
    Critical
}

[System.Serializable]
public class Effects
{
    [SerializeField] MoodId mood;
    public MoodId Status
    {
        get { return mood; }
    }
}