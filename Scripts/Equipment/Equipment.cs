using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Equipment : InteractableEntity
{
    public override void Interact(GameObject subject) {
        base.Interact(subject);
        subject.GetComponent<PlayerEquipment>().photonView.RPC("CheckItem", RpcTarget.All,photonView.ViewID);
    }


    public virtual void ApplyEffect(GameObject subject) {
        
    }

    
}
