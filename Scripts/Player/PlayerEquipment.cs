using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviourPun
{
    PlayerHealth playerHealth;
    
    private int mMaxShield;
    public int MaxShield {
        get { return mMaxShield; }
        set { 
            mMaxShield = value;
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC(nameof(ApplyMaxShield), RpcTarget.Others, mMaxShield);
            }
            if (photonView.IsMine) { 
                UIManager.instance.UpdateEquipment(1);
            }
        }
    }
    private int mCurrentShield;
    public int CurrentShield {
        get { return mCurrentShield; }
        set { 
            mCurrentShield = value;
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC(nameof(ApplyShield), RpcTarget.Others, mCurrentShield);
            }
            if (photonView.IsMine) {
                UIManager.instance.UpdateHp(playerHealth.currentHp, mCurrentShield);
            }

        }
    }

    private bool mbIsHeating;
    public bool IsHeating {
        get { return mbIsHeating; }
        set {
            mbIsHeating = value;
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC(nameof(ApplyHeating), RpcTarget.Others, mbIsHeating);
            }
            if (photonView.IsMine) {
                UIManager.instance.UpdateEquipment(3);
            }
        }
    }

    private bool mbIsCooling;
    public bool IsCooling {
        get { return mbIsCooling; }
        set {
            mbIsCooling = value;
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC(nameof(ApplyCooling), RpcTarget.Others, mbIsCooling);
            }
            if (photonView.IsMine) {
                UIManager.instance.UpdateEquipment(4);
            }
        }
    }

    private bool mbHasLight;
    public bool HasLight {
        get { return mbHasLight; }
        set {
            mbHasLight = value;
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC(nameof(ApplyLight), RpcTarget.Others, mbHasLight);
            }
            if (photonView.IsMine) {
                UIManager.instance.UpdateEquipment(2);
            }
        }
    }

    public void UnEquip(Equipment equipment) {
    }

    [PunRPC]
    public void CheckItem(int photonViewId) {
        PhotonView item = PhotonView.Find(photonViewId);
        if (item != null) {
            if (item.IsMine) {
                item.GetComponent<Equipment>().ApplyEffect(gameObject);

                Destroy(item.gameObject);
                //photonView.RPC("Equip", RpcTarget.All, item.GetComponent<Equipment>());
                PhotonNetwork.Destroy(item.gameObject);
            }
        }
    }

    [PunRPC]
    public void ApplyShield(int currentShield) {
        CurrentShield = currentShield;
    }

    [PunRPC]
    public void ApplyMaxShield(int maxShield) {
        MaxShield = maxShield;
    }


    [PunRPC]
    public void ApplyHeating(bool isHeating) {
        IsHeating = isHeating;
    }

    [PunRPC]
    public void ApplyCooling(bool isCooling) {
        IsCooling = isCooling;
    }

    [PunRPC]
    public void ApplyLight(bool hasLight) {
        HasLight = hasLight;
    }

    private void Awake() {
        playerHealth = GetComponent<PlayerHealth>();
    }
    private void OnEnable() {
        CurrentShield = mMaxShield;
    }
}
    