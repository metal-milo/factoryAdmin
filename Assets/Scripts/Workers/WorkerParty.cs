using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerParty : MonoBehaviour
{
    [SerializeField] List<Worker> workers;

    public event Action OnUpdated;

    public List<Worker> Workers
    {
        get
        {
            return workers;
        }
        set
        {
            workers = value;
            OnUpdated?.Invoke();
        }
    }

    public void ClearWorkers()
    {
        workers = new List<Worker>();
    }

    public void AddWorker(Worker todayWorker)
    {
        if (workers.Count < 6)
        {
            workers.Add(todayWorker);
        }
        else
        {
            Debug.Log("Mas de 6 trabajadores");
        }
    }

    public void PartyUpdated()
    {
        OnUpdated?.Invoke();
    }

    public static WorkerParty GetWorkerParty()
    {
        return FindObjectOfType<PlayerController>().GetComponent<WorkerParty>();
    }


}
