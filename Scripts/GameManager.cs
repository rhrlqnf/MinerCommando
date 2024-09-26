using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPun {
    public static GameManager Instance { get; private set; }

    public enum Stage {
        A_1,
        B_1,
        C_1,
        D_1,
        end
    }

    [SerializeField]
    public Stage currentStage;

    [SerializeField]
    public StageAnimationData[] stageAnimations;

    private void Awake() {
        Debug.Log("setting");
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 60;
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else {
            Destroy(this.gameObject);
        }
    }

    public void StageClear(Stage nextStage) {
        currentStage = nextStage;
    }

    public RuntimeAnimatorController GetCurrentStageAnimatorController() {
        foreach (var stageAnimation in stageAnimations) {
            if (stageAnimation.stage == currentStage) {
                return stageAnimation.animatorController;
            }
        }
        return null;
    }
}
