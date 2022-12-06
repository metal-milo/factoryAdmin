using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour
{
    [SerializeField] Text timeText;
    [SerializeField] Text ampmText;

    private void Start()
    {

    }

    public void OnEnable()
    {
        TimeController.OnMinuteChanged += SetTimeTxt;
        TimeController.OnHourChanged += SetTimeTxt;
    }

    public void OnDisable()
    {
        TimeController.OnMinuteChanged -= SetTimeTxt;
        TimeController.OnHourChanged -= SetTimeTxt;
    }

    void SetTimeTxt()
    {
        
        if (TimeController.Hour < 12)
        {
            timeText.text = $"{TimeController.Hour:00}:{TimeController.Minute:00}";
            ampmText.text = "AM";
        }
        else if (TimeController.Hour == 12)
        {
            timeText.text = $"{TimeController.Hour:00}:{TimeController.Minute:00}";
            ampmText.text = "PM";
        }
        else
        {
            timeText.text = $"{TimeController.Hour-12:00}:{TimeController.Minute:00}";
            ampmText.text = "PM";
        }
    }
}
