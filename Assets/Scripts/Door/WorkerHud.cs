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

    private void Awake()
    {
        nameText.gameObject.SetActive(true);
        levelText.gameObject.SetActive(true);
        salaryText.gameObject.SetActive(true);
        prodText.gameObject.SetActive(true);
    }

    public void SetData(Worker worker)
    {
        nameText.gameObject.SetActive(true);
        levelText.gameObject.SetActive(true);
        salaryText.gameObject.SetActive(true);
        prodText.gameObject.SetActive(true);
        nameText.text = worker.Base.Name;
        levelText.text = "Lvl " + worker.Level;
        salaryText.text = "Salary: " + worker.Salary + "/h";
        prodText.text = "Prod: " + worker.hourlyProduction + "/h";
    }

    public void activateTexts()
    {
        nameText.gameObject.SetActive(true);
        levelText.gameObject.SetActive(true);
        salaryText.gameObject.SetActive(true);
        prodText.gameObject.SetActive(true);
    }
}
