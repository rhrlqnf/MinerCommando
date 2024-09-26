using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine.UI;
using TMPro;
using ExitGames.Client.Photon;
using UnityEngine.Rendering.Universal;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    GameObject player1Prefab;
    [SerializeField]
    GameObject player2Prefab;
    [SerializeField]
    GameObject player3Prefab;
    [SerializeField] 
    GameObject player4Prefab;

    ChestStorage chestStorage;



    [SerializeField]
    private Transform[] startPositions;  // 시작 위치를 배열로 관리

    public static StageManager Instance { get; private set; }

    bool gameEnd;

    [SerializeField]
    GameManager.Stage nextStage;


    public PlayerLight[] playersLight;
    public Light2D minerShipLight;

    [SerializeField]
    public bool bShouldLight;
    [SerializeField]
    public bool bIsCold;
    public bool bIsHot;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(this.gameObject);
        }


        if (PhotonNetwork.IsMasterClient) {
            Invoke(nameof(FindPlayers), 10f);
        }
    }

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if (PhotonNetwork.IsConnectedAndReady)
        {
            SpawnPlayer();
        }
    }
    

    private void SpawnPlayer()
    {
        // 플레이어 인덱스에 따라 다른 위치에 스폰
        int index = PhotonNetwork.LocalPlayer.ActorNumber - 1;  // ActorNumber starts at 1
        index = index % startPositions.Length;  // 위치가 부족할 경우를 대비한 나머지 연산

        GameObject playerPrefab = GetPlayerPrefab(PhotonNetwork.LocalPlayer.ActorNumber);
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, startPositions[index].position, Quaternion.identity);
    }
    private GameObject GetPlayerPrefab(int actorNumber)
    {
        switch (actorNumber)
        {
            case 1:
                return player1Prefab;
            case 2:
                return player2Prefab;
            case 3:
                return player3Prefab;
            case 4:
                return player4Prefab;
            default:
                return player1Prefab;  // 기본값으로 player1Prefab을 사용
        }
    }


    //주기적으로 동기화 해야하는 데이터
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(nextStage);
            stream.SendNext(bShouldLight);
            stream.SendNext(bIsCold);
            stream.SendNext(bIsHot);
        }
        else
        {
            nextStage = (GameManager.Stage)stream.ReceiveNext();
            bShouldLight = (bool)stream.ReceiveNext();
            bIsCold = (bool)stream.ReceiveNext();
            bIsHot = (bool)stream.ReceiveNext();
        }
    }

    public void FindPlayers() {
        playersLight = FindObjectsByType<PlayerLight>(FindObjectsSortMode.None);
    }

    [PunRPC]
    public void MissionClear()
    {
        if (!gameEnd) {
            Time.timeScale = 0;
            PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate = 1;

            UIManager.instance.MissionClear();

            
                GameManager.Instance.StageClear(nextStage);
            
            gameEnd = true;
            photonView.RPC("MissionClear", RpcTarget.Others);
            
        }
    }

    [PunRPC]
    public void MissionFail() {
        if (!gameEnd) {

            Time.timeScale = 0;
            PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate = 1;

            UIManager.instance.MissionFail();

            gameEnd = true;
            photonView.RPC("MissionFail", RpcTarget.Others);
         
        }

    }

    public void BackToLobby() {
        if (PhotonNetwork.IsMasterClient) // 마스터 클라이언트인지 확인
        {
            // 현재 스테이지가 D_1인지 확인
            if (GameManager.Instance.currentStage == GameManager.Stage.end) {

                LoadEnding();
            }
            else {
                PhotonNetwork.LoadLevel("Loading for Planet to Lobby"); // 로비로 이동
            }
        }
    }


    public void LoadEnding() {
        SceneManager.LoadScene("Ending");
    }
    
}
