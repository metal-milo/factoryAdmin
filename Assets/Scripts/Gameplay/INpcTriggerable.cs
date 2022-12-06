using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INpcTriggerable
{
    void OnNpcTriggered(NPCController customer);

    void OnWorkerNpcTriggered(WorkerNPCController worker);
}

