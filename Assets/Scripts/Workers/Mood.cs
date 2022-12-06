using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mood
{
    public MoodId Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string StartMessage { get; set; }
    public Action<Worker> OnBeforeShift { get; set; }
}