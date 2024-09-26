using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// ä���� ����
public class MinedOre : InteractableEntity, IPunObservable
{
    private SpriteRenderer spRd;
    private Inventory inventory;
    private OreData oreData;
    [SerializeField]
    private SpriteRenderer outlineSpRd;

    private void Awake() // spRd�� �׻� ��ȣ�� ������ ���� Awake�� 
    {
        spRd = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        inventory = FindObjectOfType<Inventory>();

        if (oreData != null)
        {
            spRd.sprite = oreData.minedOreSprite;
            outlineSpRd.sprite = oreData.minedOreOutlineSprite;
            //상호작용 UI 색상 검정으로 변경
            if (StageManager.Instance.bIsCold) {
                outlineSpRd.color = new Color(0, 0, 0, 1);
            }
        }

    }

    public void SetUp(OreData oreData)
    {
        this.oreData = oreData;

        
    }

    public override void Interact(GameObject subject)
    {
        base.Interact(subject);
        //if (inventory.AddOre(oreData.oreId))
        //{
        //    //photonView.RPC("DestroyMinedOre", RpcTarget.AllBuffered);
        //    photonView.RPC("DestroyMinedOre", RpcTarget.All);
        //    SoundManager.Instance.PlaySfx(SoundManager.Sfx.item);
        //}
        if (inventory.CheckSlot(oreData.oreId)) {
            subject.GetComponent<PlayerStorageSystem>().photonView.RPC("CheckMinedOre", RpcTarget.All, photonView.ViewID, oreData.oreId);
        }
    }



    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(oreData.oreId);
        }
        else
        {
            int oreId = (int)stream.ReceiveNext();
            oreData = FindOreDataById(oreId);
            if (oreData != null)
            {
                spRd.sprite = oreData.minedOreSprite;
                outlineSpRd.sprite = oreData.minedOreOutlineSprite;
            }
        }
    }

    private OreData FindOreDataById(int oreId)
    {
        // ScriptableDatas �������� OreData�� �ε��Ͽ� ã��
        OreData[] oreDataArray = Resources.LoadAll<OreData>("ScriptableDatas");
        foreach (OreData data in oreDataArray)
        {
            if (data.oreId == oreId)
            {
                return data;
            }
        }
        return null;
    }
}