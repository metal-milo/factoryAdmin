using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    PartyMemberUI[] memberSlots;
    int selection = 0;
    List<Worker> workers;
    WorkerParty party;
    public Worker SelectedMember => workers[selection];

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);

        party = WorkerParty.GetWorkerParty();
        workers = party.Workers;
        SetPartyData(workers);
    }

    public void SetPartyData(List<Worker> workers)
    {
        
        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < workers.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].SetData(workers[i]);
            }
                
            else
                memberSlots[i].gameObject.SetActive(false);
        }
        UpdateMemberSelection(selection);

    }

    public void HandleUpdate(Action onBack, Action onSelected = null)
    {
        var prevSelection = selection;

        if (Input.GetKeyDown(KeyCode.D))
            ++selection;
        else if (Input.GetKeyDown(KeyCode.A))
            --selection;
        else if (Input.GetKeyDown(KeyCode.S))
            selection += 2;
        else if (Input.GetKeyDown(KeyCode.W))
            selection -= 2;

        selection = Mathf.Clamp(selection, 0, workers.Count - 1);

        if (selection != prevSelection)
            UpdateMemberSelection(selection);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            onSelected?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
        }
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < workers.Count; i++)
        {
            if (i == selectedMember)
                memberSlots[i].SetSelected(true);
            else
                memberSlots[i].SetSelected(false);
        }
    }

}
