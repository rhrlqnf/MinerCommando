using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using Photon.Pun;

public class Mission : MonoBehaviourPun
{
    [SerializeField]
    Request[] requests;

    [SerializeField]
    ChestStorage chestStorage;
    bool missionComplete;

    public static Mission instance {
        get {
            if (m_instance == null) {
                m_instance = FindObjectOfType<Mission>(true);
            }

            return m_instance;
        }
    }
    private static Mission m_instance;

    private void Awake() { 
        if (instance != this) {
            Destroy(gameObject);
        }
    }
#if UNITY_EDITOR
    private void OnValidate() {
        requests = transform.GetComponentsInChildren<Request>();
    }
#endif

    private void Start() {
        chestStorage = FindObjectOfType<ChestStorage>(true);
        UpdateMissionUI();
    }


    public void UpdateMissionUI() {
        for (int i = 0; i < requests.Length; i++) {
            if (requests[i] != null) {
                requests[i].collectedOreCount = chestStorage.CountOre(requests[i].targetOreData.oreId);
            }
            else {

            }
        } 
    }
    public void CheckMissionComplete() {
        missionComplete = true;

        for (int i = 0; i < requests.Length; i++) {
            if (requests[i] != null && requests[i].isSatisfied) {
            }
            else {
                missionComplete = false;
            }
        }

        if(missionComplete == true) {
            StageManager.Instance.MissionClear();
        }
    }
}
