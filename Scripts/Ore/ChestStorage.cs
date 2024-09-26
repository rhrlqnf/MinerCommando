using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChestStorage : Storage, IPunObservable
{
    public override bool AddOre(int oreId)
    {
        bool result = base.AddOre(oreId);
        if (result)
        {
            photonView.RPC("RPC_AddOre", RpcTarget.OthersBuffered, oreId);
            Mission.instance.UpdateMissionUI();
        }
        return result;
    }

    public override void RemoveOre(int slotIndex)
    {
        base.RemoveOre(slotIndex);   
        photonView.RPC("RPC_RemoveOre", RpcTarget.OthersBuffered, slotIndex);
        Mission.instance.UpdateMissionUI();
    }

    [PunRPC]
    private void RPC_AddOre(int oreId)
    {
        base.AddOre(oreId);
    }

    [PunRPC]
    private void RPC_RemoveOre(int slotIndex)
    {
        base.RemoveOre(slotIndex);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 데이터를 전송하는 측 :  호스트
            foreach (Slot slot in slots)
            {
                stream.SendNext(slot.oreData != null ? slot.oreData.oreId : -1);
                stream.SendNext(slot.count);
            }
            Mission.instance.UpdateMissionUI();


        }
        else
        {
            // 데이터를 수신하는 측 : 클라이언트
            for (int i = 0; i < slots.Length; i++)
            {
                int oreId = (int)stream.ReceiveNext();
                int count = (int)stream.ReceiveNext();

                if (slots[i] == null)
                {
                    slots[i] = new GameObject("Slot").AddComponent<Slot>();
                    slots[i].transform.SetParent(transform); // 부모 객체 설정
                }

                slots[i].oreId = oreId;  // oreId를 설정
                slots[i].count = count;  // count를 설정하면서 oreData를 초기화
                slots[i].oreData = FindOreDataById(oreId);
            }
            Mission.instance.UpdateMissionUI();

        }
    }
    private OreData FindOreDataById(int oreId)
    {
        // ScriptableDatas 폴더에서 OreData를 로드하여 찾기
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

    public int CountOre(int oreId) {
        int count = 0;
        foreach(Slot slot in slots) {

            if(slot.oreData!=null && slot.oreData.oreId == oreId) {
                count += slot.count;
            }
        }

        return count;
    }


    // 추가된 코드 : 클라이언트가 처음 창고 열때 비동기화 문제를 해결하기 위해
    //호스트가 자원을 넣고 나온 후 클라가 창고를 열었을 때 동기화가 안됨, 호스트가 창고를 열고 있는 상태에서
    //클라이언트가 열면 그때 부터 동기화됨(이후도 됨)
    //그래서 초기화 문제로 인식하고 이에 필요한 코드 작성
    public void RequestInitialState() {
        photonView.RPC("RPC_RequestInitialState", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_RequestInitialState(PhotonMessageInfo info) {
        foreach (Slot slot in slots) {
            int oreId = slot.oreData != null ? slot.oreData.oreId : -1;
            photonView.RPC("RPC_ReceiveInitialState", info.Sender, oreId, slot.count);
        }
    }

    [PunRPC]
    private void RPC_ReceiveInitialState(int oreId, int count) {
        for (int i = 0; i < slots.Length; i++) {
            if (slots[i].oreData != null && slots[i].oreData.oreId == oreId) {
                slots[i].count = count;
                slots[i].oreData = FindOreDataById(oreId);
                return;
            }

            if (slots[i].oreData == null) {
                slots[i].oreId = oreId;
                slots[i].count = count;
                slots[i].oreData = FindOreDataById(oreId);
                return;
            }
        }
    }

    //애는 필요없응 코드, 일단 내비둠
    public void ClearAllSlot() {
        for(int i =0; i< slots.Length;i++) {
            slots[i].count = 0;
        }
    }


    public bool CheckSlot(int slotIndex, int oreId) {
        if (GetOreId(slotIndex) == oreId && slots[slotIndex].count > 0) {
            return true;
        }
        return false;
    }

}
