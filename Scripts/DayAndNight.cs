using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayAndNight : MonoBehaviourPun,IPunObservable
{
    const int DAY_DURATION = 180;
    const int NIGHT_DURATION = 120;
    const float DELTA_LIGHT = 0.001f;

    [SerializeField]
    GameObject nightOre;

    Light2D dayLight;
    float mLightIntensity = 1f;
    public float LightIntensity {
        get { return mLightIntensity; }
        set {
            mLightIntensity = value;
            dayLight.intensity = mLightIntensity;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(mLightIntensity);
        }
        else {
            LightIntensity = (float)stream.ReceiveNext();
        }
    }

    private void Awake() {
        dayLight = GetComponent<Light2D>();
    }
    private void Start() {
        if(PhotonNetwork.IsMasterClient) {
            Invoke(nameof(StartSunSetCoroutine), DAY_DURATION);
            Debug.Log("start");
        }
    }

    IEnumerator SunSet() {
        StageManager.Instance.bShouldLight = true;
        StageManager.Instance.bIsHot = false;

        foreach (PlayerLight playerLight in StageManager.Instance.playersLight) {
            playerLight.photonView.RPC("LightOn",RpcTarget.All);
        }
        photonView.RPC(nameof(ShipLightOn), RpcTarget.All);

        while (mLightIntensity > 0) {
            LightIntensity = Mathf.MoveTowards(mLightIntensity, 0, DELTA_LIGHT);
            Debug.Log("sunset");
            yield return null;
        }
        photonView.RPC(nameof(NightOreOn), RpcTarget.All);

        Invoke(nameof(StartSunRiseCoroutine), NIGHT_DURATION);
        yield break;
    }

    IEnumerator SunRise() {
        photonView.RPC(nameof(NightOreOff), RpcTarget.All);

        while (mLightIntensity < 1) {
            LightIntensity = Mathf.MoveTowards(mLightIntensity, 1, DELTA_LIGHT);
            yield return null;
        }
        StageManager.Instance.bShouldLight = false;
        StageManager.Instance.bIsHot = true;

        foreach (PlayerLight playerLight in StageManager.Instance.playersLight) {
            playerLight.photonView.RPC("LightOff", RpcTarget.All);
        }
        photonView.RPC(nameof(ShipLightOff), RpcTarget.All);

        Invoke(nameof(StartSunSetCoroutine), DAY_DURATION);
        yield break;
    }


    void StartSunSetCoroutine() {
        StartCoroutine(nameof(SunSet));
        Debug.Log("coroutine start");
    }

    void StartSunRiseCoroutine() {
        StartCoroutine(nameof(SunRise));
    }

    [PunRPC]
    void ShipLightOn() {
        StageManager.Instance.minerShipLight.intensity = 1;
    }

    [PunRPC]
    void ShipLightOff() {
        StageManager.Instance.minerShipLight.intensity = 0;
    }

    [PunRPC]
    void NightOreOn() {
        nightOre.SetActive(true);
    }
    [PunRPC]
    void NightOreOff() {
        nightOre.SetActive(false);
    }
}
