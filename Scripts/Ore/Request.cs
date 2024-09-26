using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class Request: MonoBehaviourPunCallbacks {
    public OreData targetOreData;


    [SerializeField]
    Image targetOreImage;

    [SerializeField]
    TMP_Text targetOreCount;

    [SerializeField]
    float requiredOreCount;

    private float _collectedOreCount;
    public float collectedOreCount {
        get { return _collectedOreCount; }
        set {
            _collectedOreCount = value;
            targetOreCount.text = _collectedOreCount + "/" + requiredOreCount;

            if (_collectedOreCount >= requiredOreCount) {
                if (!isSatisfied) {
                    isSatisfied = true;
                    missionCompleteImage.sprite = satisfiedImage;
                    Mission.instance.CheckMissionComplete();
                }
            }
            else {
                isSatisfied = false;
                missionCompleteImage.sprite = unsatisfiedImage;
            }
        }
    }



    public bool isSatisfied;

    [SerializeField]
    Image missionCompleteImage;

    [SerializeField]
    Sprite satisfiedImage, unsatisfiedImage;

    private void Start() {
        targetOreImage.sprite = targetOreData.minedOreSprite;
    }



}
