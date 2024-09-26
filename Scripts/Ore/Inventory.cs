using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
//using TreeEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Inventory : Storage
{
    // 플레이어 인벤토리 전용 기능 구현
    public override bool AddOre(int oreId)
    {
        bool result = base.AddOre(oreId);
        if (result)
        {
            Debug.Log("added");
        }
        return result;
    }

    public override void RemoveOre(int slotIndex)
    {
        base.RemoveOre(slotIndex);
        Debug.Log("removed");
    }

    public void DropOre(Vector2 dropPos) {
        for(int i = 0; i<slots.Length; i++) {
            if (slots[i].oreData != null) {
                int count = slots[i].count;
                for(int j = 0; j < count; j++) {
                    //Vector2 minedOrePos = dropPos + new Vector2(UnityEngine.Random.Range(-2, 3), UnityEngine.Random.Range(-2, 3));
                    Vector2 randomPos = Random.insideUnitCircle * 2 + dropPos;
                    NavMeshHit hit;
                    NavMesh.SamplePosition(randomPos, out hit, 2, NavMesh.AllAreas);
                    Vector2 position = hit.position;
                    if (float.IsInfinity(position.x) || float.IsInfinity(position.y)) {
                        position = dropPos;
                    }

                    GameObject minedOre = PhotonNetwork.Instantiate("MinedOre", position, Quaternion.identity);
                    MinedOre minedOreComponent = minedOre.GetComponent<MinedOre>();
                    if (minedOreComponent != null) {
                        minedOreComponent.SetUp(slots[i].oreData);
                    }
                    RemoveOre(i);
                }
            }
        }
    }

    public bool CheckSlot(int oreId) {
        foreach (Slot slot in slots) {
            if (slot.oreData != null) {
                if (slot.oreData.oreId == oreId && slot.count < slot.maxCapacity) {
                    return true;
                }
            }
        }
        foreach (Slot slot in slots) {
            if (slot.oreData == null) {
                return true;
            }
        }
        Debug.Log("slot is full");
        return false;
    }
}
