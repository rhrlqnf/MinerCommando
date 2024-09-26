using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

//ScriptableObject
[CreateAssetMenu(menuName = "Scriptable/OreData", fileName = "Ore Data")]
public class OreData : ScriptableObject
{
    public string oreName;
    public int oreId;
    public int hitsRequired;
    public Sprite minedOreSprite;
    public Sprite minedOreOutlineSprite;

    public int minAmount, maxAmount;
}
