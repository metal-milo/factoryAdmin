using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class humanR : MonoBehaviour
{
    List<Worker> outsideWorkers = new List<Worker>();
    

    private void Awake()
    {
        FillWorkerList();
    }

    public Worker GetRandomWorker(int workerIndex)
    {
        
        return outsideWorkers[workerIndex];
        //var outsideWorker = outsideWorkers[workerIndex];
        //outsideWorker.Init();
        //return outsideWorker;
    }

    void FillWorkerList()
    {
        Dictionary<string, WorkerBase> objects = WorkerDB.objects;
        foreach (var objectWorker in objects)
        {
            var outsideWorkerCopy = new Worker(objectWorker.Value, 11);
            outsideWorkers.Add(outsideWorkerCopy);
        }
    }
}
