using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LightHelmet : Equipment {
    public override void ApplyEffect(GameObject subject) {
        subject.GetComponent<PlayerEquipment>().HasLight = true;
    }
}