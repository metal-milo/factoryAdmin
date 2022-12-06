using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkerHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text salaryText;
    [SerializeField] Text prodText;

    public void SetData(Worker worker)
    {
        nameText.text = worker.Base.Name;
        levelText.text = "Lvl " + worker.Level;
        salaryText.text = "Salary: " + worker.Salary + "/h";
        prodText.text = "Prod: " + worker.hourlyProduction + "/h";
    }
}
