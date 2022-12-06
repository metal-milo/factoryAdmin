using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour, ISavable
{

    public float moveSpeed;

    private bool isMoving;
    private Vector2 input;

    private Animator animator;

    public LayerMask interactableLayer;

    public LayerMask solidObjectLayer;

    public LayerMask portalLayer;

    public float OffsetY { get; private set; } = 0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);
                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;
                if (IsWalkable(targetPos))
                    StartCoroutine(Move(targetPos));
            }
        }
        animator.SetBool("isMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.Z))
            StartCoroutine(Interact());
            
    }

    IEnumerator Interact()
    {
        var facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        var interactPos = transform.position + facingDir;

        //Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);
        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, interactableLayer);
        if (collider != null)
        {
            yield return collider.GetComponent<Interactable>()?.Interact(transform);

        }
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        isMoving = false;
        OnMoveOver();
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectLayer | interactableLayer) != null)
        {
            return false;
        }

        return true;
    }

    private void OnMoveOver()
    {
        var facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        var interactPos = transform.position + facingDir;
        var colliders = Physics2D.OverlapCircleAll(interactPos, 0.2f, portalLayer);
        foreach (var collider in colliders)
        {
            var triggerable = collider.GetComponent<IPlayerTriggerable>();
            if (triggerable != null)
            {
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }
    }

    public void StopMoving()
    {
        isMoving = false;
    }

    public void SetPositionAndSnapToTile(Vector2 pos)
    {
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f + OffsetY;

        transform.position = pos;
    }

    public object CaptureState()
    {
        var saveData = new PlayerSaveData()
        {
            position = new float[] { transform.position.x, transform.position.y },
            workers = GetComponent<WorkerParty>().Workers.Select(p => p.GetSaveData()).ToList()
        };

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (PlayerSaveData)state;

        // Restore Position
        var pos = saveData.position;
        transform.position = new Vector3(pos[0], pos[1]);

        // Restore Party revisar!!
        GetComponent<WorkerParty>().Workers = saveData.workers.Select(s => new Worker(s)).ToList();
    }
}

[Serializable]
public class PlayerSaveData
{
    public float[] position;
    public List<WorkerSaveData> workers;
}
