using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimState { Idle, Moving }
public class AnimController : MonoBehaviour
{

    AnimState state;
    float idleTimer = 0f;
    int currentPattern = 0;

    [SerializeField] List<Vector2> movementPattern;
    SomethingAnimated somethingAnimated;
    private void Awake()
    {
        somethingAnimated = GetComponent<SomethingAnimated>();
        state = AnimState.Idle;
    }

    

    private void Update()
    {
        if (state == AnimState.Idle)
        {

            StartCoroutine(IdleMove());
        }

        somethingAnimated.HandleUpdate();
    }
    
    IEnumerator IdleMove()
    {

        var oldPos = transform.position;

        yield return somethingAnimated.IdleMove();

        state = AnimState.Idle;
    }
    
    
    
}

