using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLight : MonoBehaviourPun
{
    PlayerEquipment playerEquipment;
    [SerializeField]
    Light2D playerLight;


    [PunRPC]
    private void LightOn() {
        if (playerEquipment.HasLight) {
            playerLight.pointLightOuterRadius = 4;
        }
        playerLight.intensity = 1f;
    }

    [PunRPC]
    private void LightOff() {
        playerLight.intensity = 0;
    }

    private void Awake() {
        playerEquipment = GetComponent<PlayerEquipment>();
    }
}
