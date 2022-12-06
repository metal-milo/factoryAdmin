using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Option
{
    public OptionBase Base { get; set; }
    public float Acuraccy { get; set; }

    public Option(OptionBase pBase)
    {
        Base = pBase;
        Acuraccy = pBase.Accuracy;
    }

    public Option(OptionSaveData saveData)
    {
        Base = OptionDB.GetObjectByName(saveData.name);
        Acuraccy = saveData.accuracy;
    }

    public OptionSaveData GetSaveData()
    {
        var saveData = new OptionSaveData()
        {
            name = Base.name,
            accuracy = Acuraccy
        };
        return saveData;
    }
}

[Serializable]
public class OptionSaveData
{
    public string name;
    public float accuracy;
}
