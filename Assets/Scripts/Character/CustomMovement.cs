using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[System.Serializable]
public class CustomMovement
{

    Vector2 moveCharacter { get; set; }
    float waitTime { get; set; }

    public CustomMovement(Vector2 moveCharacter, float waitTime)
    {
        this.moveCharacter = moveCharacter;
        this.waitTime = waitTime;
    }


}
