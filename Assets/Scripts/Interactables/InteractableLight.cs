using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InteractableLight : MonoBehaviour, Interactable
{
    public event Action OnDoor;
    [SerializeField] Dialog dialog;
    public IEnumerator Interact(Transform initiator)
    {
        OnDoor();
        yield return null;
    }
}
