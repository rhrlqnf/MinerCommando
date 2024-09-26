using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStorageSystem : MonoBehaviourPun
{
    private ChestStorage chestStorage;
    private Inventory inventory;
    private void Start()
    {
        chestStorage = FindObjectOfType<ChestStorage>(true);
        inventory = FindObjectOfType<Inventory>();
    }


    //Ŭ���̾�ƮB�� �÷��̾�b�� ���ڿ��� �������� �������� �ó�����
    //CHECKSLOT�ϴ� �κа� AddOre�ϴ� �κ��� 2���� rpc �Լ��� ������
    //CheckSlot�� MASTER CLIENT�� b�� �����ϵ��� rpc ȣ���Ѵ�
    ///MASTER CLIENT�� b�� CHECKSLOT���� b�� ��û�� �������� ���Կ� �ִ����� ������ 0�� �Ѵ����� Ȯ���Ѵ�
    //�ٽ� RPC�� ��� Ŭ���̾�Ʈ�� �÷��̾� b���� AddOre�� ��Ų��
    //isMine���� B������ AddOre�� ����ȴ�
    public void TakeItem(int slotIndex, int oreId)
    {
        if (chestStorage.CheckSlot(slotIndex, oreId))
        {
            if (inventory.AddOre(oreId)) {
                chestStorage.RemoveOre(oreId);
            }
        }
    }

    [PunRPC]
    public void CheckMinedOre(int photonViewId, int oreId) {
        PhotonView ore = PhotonView.Find(photonViewId);
        if (ore != null) {
            if (ore.IsMine) {
                Destroy(ore.gameObject);
                photonView.RPC("GetOre", RpcTarget.All, oreId);
                PhotonNetwork.Destroy(ore.gameObject);
            }
        }
    }

    [PunRPC]
    private void GetOre(int oreId) {
        if (photonView.IsMine) {
            inventory.AddOre(oreId);
        }
    }
}
