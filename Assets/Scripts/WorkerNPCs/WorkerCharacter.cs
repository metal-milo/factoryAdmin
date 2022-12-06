using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerCharacter : MonoBehaviour
{
    public float moveSpeed;

    public bool IsMoving { get; private set; }

    public float OffsetY { get; private set; } = 0f;

    public bool isWorking { get; set; }

    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    public IEnumerator Move(Vector2 moveVec, Action OnMoveOver = null)
    {

        var targetPos = transform.position;
        targetPos.x += moveVec.x;
        targetPos.y += moveVec.y;




        IsMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        IsMoving = false;

        OnMoveOver?.Invoke();
    }

    public void HandleUpdate()
    {
        animator.SetBool("IsWorking", isWorking);
    }

    public void StopMoving()
    {
        isWorking = false;
    }

    public void StartMoving()
    {
        isWorking = true;
    }

    
    public void Visible(bool isVisible)
    {
        gameObject.SetActive(isVisible);
        if (isVisible)
            StartMoving();
        else
            StopMoving();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Need to be destroyed");
        Destroy(gameObject);
    }
}

