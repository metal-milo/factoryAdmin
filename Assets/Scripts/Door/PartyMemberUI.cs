using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text salaryText;

    Worker _worker;

    public void SetData(Worker worker)
    {
        _worker = worker;

        nameText.text = worker.Base.Name;
        levelText.text = "Lvl " + worker.Level;
        salaryText.text = "Salary: " + worker.Salary;
    }

    

    public void SetSelected(bool selected)
    {
        if (selected)
            nameText.color = GlobalSettings.i.HighlightedColor;
        else
            nameText.color = Color.black;
    }
}
