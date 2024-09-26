using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTemperature : MonoBehaviourPun {
    PlayerHealth playerHealth;
    PlayerEquipment playerEquipment;

    const float BASE_TEMPERATURE = 20f;
    const float MIN_TEMPERATURE = -20f;
    const float MAX_TEMPERATURE = 60f;
    const float FREEZING_TEMPERATURE = 0f;
    const float OVERHEATING_TEMPERATURE = 40f;

    const float TIME_BET_DAMAGED = 4f;

    private float mLastDamagedTime;


    [SerializeField]
    private float mTemperature = 20f;
    public float Temperature {
        get { 
            return mTemperature; 
        }
        set {
            if (value < MIN_TEMPERATURE) {
                mTemperature = MIN_TEMPERATURE;
            }
            else if (value > MAX_TEMPERATURE) {
                mTemperature = MAX_TEMPERATURE;
            }
            else {
                mTemperature = value;
            }
            if (photonView.IsMine) {
                UIManager.instance.UpdateTemperature(mTemperature);
            }
            Debug.Log("Temperature called");
        }
    }

    public bool IsFreezing {
        get{
            if (mTemperature < FREEZING_TEMPERATURE) {
                return true;
            }
            return false;
        }
    }

    public bool IsOverheating {
        get {
            if (mTemperature > OVERHEATING_TEMPERATURE) {
                return true;
            }
            return false;
        }
    }

    //void DecreaseTemperature() {

    //    if (mTemperature - delta < MIN_TEMPERATURE) {
    //        mTemperature = MAX_TEMPERATURE;
    //    }
    //    mTemperature -= delta;
    //}

    //void IncreaseTemperature(float delta) {
    //    if (mTemperature + delta > MAX_TEMPERATURE) {
    //        mTemperature = MAX_TEMPERATURE;
    //    }
    //    mTemperature += delta;
    //}
    private void Awake() {
        playerHealth = GetComponent<PlayerHealth>();
        playerEquipment= GetComponent<PlayerEquipment>();
    }

    private void OnEnable() {
        Temperature = BASE_TEMPERATURE;
    }
    private void Update() {
        if(photonView.IsMine && (IsFreezing || IsOverheating) && Time.time > mLastDamagedTime + TIME_BET_DAMAGED) {
            photonView.RPC("OnDamage",RpcTarget.MasterClient, 2);
            mLastDamagedTime = Time.time;
        }
    }
    private void FixedUpdate() {
        if (photonView.IsMine) {
            if (StageManager.Instance.bIsCold) {
                Temperature -= 0.02f;
            }
            else if (StageManager.Instance.bIsHot) {
                Temperature += 0.02f;
            }
            else {
                Temperature = Mathf.MoveTowards(mTemperature, BASE_TEMPERATURE, 0.1f);
            }
        }
        if (playerEquipment.IsHeating || playerEquipment.IsCooling) {
            Temperature = Mathf.MoveTowards(mTemperature, BASE_TEMPERATURE, 0.1f);
        }
    }
}
