using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TriggerDestroyer : MonoBehaviour, INpcTriggerable
{
    public static Action destroyCust;
    public void OnNpcTriggered(NPCController customer)
    {
        //Debug.Log("Destroy NPC");
        //Destroy(GameObject.Find("Customer(Clone)"));
    }

    public void OnWorkerNpcTriggered(WorkerNPCController worker)
    {
        throw new NotImplementedException();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Destroy NPC");
        Destroy(other.gameObject);
    }
}
