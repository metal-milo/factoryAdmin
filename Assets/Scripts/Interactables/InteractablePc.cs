using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablePc : MonoBehaviour, Interactable
{
    [SerializeField] List<ItemBase> rawMaterials;

    public List<ItemBase> RawMaterials => rawMaterials;

    public IEnumerator Interact(Transform initiator)
    {
        yield return ShopController.i.StartTrading(this);
    }

}