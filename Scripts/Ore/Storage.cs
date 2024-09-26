using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Storage : MonoBehaviourPun
{
    [SerializeField]
    protected Slot[] slots;

#if UNITY_EDITOR
    private void OnValidate()
    {
        slots = transform.GetComponentsInChildren<Slot>();
    }
#endif

    public virtual bool AddOre(int oreId)
    {
        foreach (Slot slot in slots)
        {
            if (slot.oreData != null)
            {
                if (slot.oreData.oreId == oreId && slot.count < slot.maxCapacity)
                {
                    slot.count++;
                    return true;
                }
            }
        }
        foreach (Slot slot in slots)
        {
            if (slot.oreData == null)
            {
                slot.oreData = ItemManager.ores[oreId];
                slot.count++;
                return true;
            }
        }
        Debug.Log("slot is full");
        return false;
    }

    public virtual void RemoveOre(int slotIndex)
    {
        slots[slotIndex].count--;

        if (slots[slotIndex].count < 0)
        {
            slots[slotIndex].count = 0;
        }
    }

    public int GetOreId(int slotIndex)
    {
        if (slots[slotIndex].oreData != null)
        {
            return slots[slotIndex].oreData.oreId;
        }
        else
        {
            return -1;
        }
    }

}
