using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;

    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;

    [SerializeField] QuestBase questToStart;
    [SerializeField] QuestBase questToComplete;

    ItemGiver itemGiver;
    Quest activeQuest;
    NPCState state;
    float idleTimer = 0f;
    int currentPattern = 0;


    Character character;
    private void Awake()
    {
        character = GetComponent<Character>();
        itemGiver = GetComponent<ItemGiver>();
    }

    public IEnumerator Interact(Transform initiator)
    {
        if (state == NPCState.Idle)
        {
            state = NPCState.Dialog;
            character.LookTowards(initiator.position);

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
            state = NPCState.Idle;
        }
    }

    private void Update()
    {
        if (state == NPCState.Idle)
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
        state = NPCState.Walking;

        var oldPos = transform.position;

        yield return character.Move(movementPattern[currentPattern], OnMoveOver);

        if (transform.position != oldPos)
            currentPattern = (currentPattern + 1) % movementPattern.Count;

        state = NPCState.Idle;
    }

    private void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, character.OffsetY), 0.2f, GameLayers.i.DestroyerLayer);
        foreach (var collider in colliders)
        {
            var triggerable = collider.GetComponent<INpcTriggerable>();
            if (triggerable != null)
            {
                triggerable.OnNpcTriggered(this);
                break;
            }
        }
    }

}



public enum NPCState { Idle, Walking, Dialog }