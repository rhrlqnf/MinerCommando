using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Armor : Equipment {
    public override void ApplyEffect(GameObject subject) {
        subject.GetComponent<PlayerEquipment>().MaxShield = 2;
        subject.GetComponent<PlayerEquipment>().CurrentShield = 2;
    }
}