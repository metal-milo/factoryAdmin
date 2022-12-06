using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SomethingAnimated : MonoBehaviour
{
    public float moveSpeed;


    public bool IsMoving { get; private set; }

    public float OffsetY { get; private set; } = 0f;

    SprAnimator animator;
    private void Awake()
    {
        animator = GetComponent<SprAnimator>();
        SetPositionAndSnapToTile(transform.position);
    }

    public void SetPositionAndSnapToTile(Vector2 pos)
    {
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f + OffsetY;

        transform.position = pos;
    }

    public IEnumerator IdleMove(Action OnMoveOver = null)
    {
        animator.IsMoving = true;
        yield return null;

    }
    public IEnumerator Move(Vector2 moveVec, Action OnMoveOver = null)
    {

        animator.MoveX = Mathf.Clamp(moveVec.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVec.y, -1f, 1f);

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
        animator.IsMoving = IsMoving;
    }

    

    public SprAnimator Animator
    {
        get => animator;
    }

    
}
