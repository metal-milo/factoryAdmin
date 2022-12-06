using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{

    public static int Minute { get; private set; }
    public static int Hour { get; private set; }

    public static Action OnMinuteChanged;
    public static Action OnHourChanged;

    private float minuteToRealTime = 0.5f;
    private float timer;

    public void Init()
    {
        Hour = 7;
        Minute = 0;
        timer = minuteToRealTime;
    }

    public void HandleUpdate()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            Minute++;
            OnMinuteChanged?.Invoke();
            if (Minute >= 60)
            {
                Hour++;
                Minute = 0;
                OnHourChanged?.Invoke();
            }
            timer = minuteToRealTime;
        }
    }

    public void SetTime(int hour, int minute)
    {
        Hour = hour;
        Minute = minute;
        timer = minuteToRealTime;
        OnMinuteChanged?.Invoke();
        OnHourChanged?.Invoke();
    }
}
