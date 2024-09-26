using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Scriptable Object로 만든 광석 데이터들을 배열로 저장
//아이디와 배열 인덱스가 일치
//이름을 매니저가 아니라 다른 것으로 바꿔야할듯
public class ItemManager : MonoBehaviour {
    public static OreData[] ores;

    private void Start() {
        ores = Resources.LoadAll<OreData>("ScriptableDatas");
    }
}
