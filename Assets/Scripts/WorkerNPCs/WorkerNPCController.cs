using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerNPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;

    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;

    [SerializeField] QuestBase questToStart;
    [SerializeField] QuestBase questToComplete;

    ItemGiver itemGiver;
    Quest activeQuest;
    WorkerNPCState state;
    float idleTimer = 0f;
    int currentPattern = 0;


    WorkerCharacter character;
    private void Awake()
    {
        character = GetComponent<WorkerCharacter>();
        itemGiver = GetComponent<ItemGiver>();
    }

    public IEnumerator Interact(Transform initiator)
    {
        if (state == WorkerNPCState.Idle)
        {
            state = WorkerNPCState.Dialog;

            if (questToComplete != null)
            {
                var quest = new Quest(questToComplete);
                yield return quest.CompleteQuest(initiator);
                questToComplete = null;

                Debug.Log($"{quest.Base.Name} completed");
            }

            if (itemGiver != null && itemGiver.CanBeGiven())
            {
                yield return itemGiver.GiveItem(initiator.GetComponent<PlayerController>());
            }
            else if (questToStart != null)
            {
                activeQuest = new Quest(questToStart);
                yield return activeQuest.StartQuest();
                questToStart = null;

                if (activeQuest.CanBeCompleted())
                {
                    yield return activeQuest.CompleteQuest(initiator);
                    activeQuest = null;
                }
            }
            else if (activeQuest != null)
            {
                if (activeQuest.CanBeCompleted())
                {
                    yield return activeQuest.CompleteQuest(initiator);
                    activeQuest = null;
                }
                else
                {
                    yield return DialogManager.Instance.ShowDialog(activeQuest.Base.InProgressDialogue);
                }
            }
            else
            {
                yield return DialogManager.Instance.ShowDialog(dialog);
            }



            idleTimer = 0f;
            state = WorkerNPCState.Idle;
        }
    }

    private void Update()
    {
        if (state == WorkerNPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > timeBetweenPattern)
            {
                idleTimer = 0f;
                if (movementPattern.Count > 0)
                    StartCoroutine(Walk());
            }
        }

        character.HandleUpdate();
    }

    IEnumerator Walk()
    {
        state = WorkerNPCState.Walking;

        var oldPos = transform.position;

        yield return character.Move(movementPattern[currentPattern], OnMoveOver);

        if (transform.position != oldPos)
            currentPattern = (currentPattern + 1) % movementPattern.Count;

        state = WorkerNPCState.Idle;
    }

    private void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, character.OffsetY), 0.2f, GameLayers.i.DestroyerLayer);
        foreach (var collider in colliders)
        {
            var triggerable = collider.GetComponent<INpcTriggerable>();
            if (triggerable != null)
            {
                triggerable.OnWorkerNpcTriggered(this);
                break;
            }
        }
    }
}

public enum WorkerNPCState { Idle, Walking, Dialog }