using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

//����
public class Ore : InteractableEntity
{
    //���� ������
    [SerializeField]
    private OreData oreData;

    //ä���� �ʿ��� ��ȣ�ۿ� Ƚ��
    private int hitCount;
    //����ϴ� ä���� ������ ��
    private int amount;

    //ä���� ���� prefab
    [SerializeField]
    private GameObject minedOrePrefab;

    private void Start()
    {
        amount = new System.Random().Next(oreData.minAmount, oreData.maxAmount + 1);
    }
   
      
    
            
    private void Update()
    {
        if (hitCount >= oreData.hitsRequired)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                OnMiningComplete();
            }
        }
    }

    //��ȣ�ۿ��� �÷��̾ ��̸� �ֵθ����� ��
    public override void Interact(GameObject subject)
    {
        base.Interact(subject);
        //subject �ִϸ��̼� �۵�
        //ä�� ���൵ ui? 
        subject.GetComponent<PlayerInteract>().SwingPickaxe(this);
    }

    //������� ����
    public void OnHit()
    {
        hitCount++;
    }

    //ä�� �Ϸ�

    private void OnMiningComplete()
    {
        for (int i = 0; i < amount; i++)
        {
            //Vector2 minedOrePos = (Vector2)transform.position + new Vector2(UnityEngine.Random.Range(-2, 3), UnityEngine.Random.Range(-2, 3));
            Vector2 randomPos = UnityEngine.Random.insideUnitCircle * 2 + (Vector2)transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomPos, out hit, 2, NavMesh.AllAreas);
            GameObject minedOre = PhotonNetwork.Instantiate(minedOrePrefab.name, hit.position, Quaternion.identity);
            MinedOre minedOreComponent = minedOre.GetComponent<MinedOre>();
            if (minedOreComponent != null)
            {
                minedOreComponent.SetUp(oreData);
            } // ���� �� �ٽ� �¾�
        }

        PhotonNetwork.Destroy(gameObject);  // ȣ��Ʈ������ ������Ʈ ����
    }

    [PunRPC]
    private void SyncMiningComplete()
    {
        gameObject.SetActive(false); // Ŭ���̾�Ʈ���� ������Ʈ ��Ȱ��ȭ
    }

    private void CallMiningComplete()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("OnMiningComplete", RpcTarget.All);
        }
        else
        {
            photonView.RPC("SyncMiningComplete", RpcTarget.All);
        }
    }

}
