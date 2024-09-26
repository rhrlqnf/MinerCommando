using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credit : MonoBehaviourPunCallbacks {
    public float scrollSpeed = 100f;
    private RectTransform RectTransform;
    private bool stopScrolling = false;

    void Start() {
        RectTransform = GetComponent<RectTransform>();
    }

    void Update() {
        // 스크롤이 멈추지 않았을 때만 스크롤 실행
        if (!stopScrolling) {
            RectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
        }

        // 텍스트가 특정 위치에 도달하면 스크롤 정지
        if (RectTransform.anchoredPosition.y > 950) {
            stopScrolling = true;  // 스크롤 정지
        }

        // P 키를 눌렀을 때 Photon 네트워크 연결 끊기 시도
        if (Input.GetKeyDown(KeyCode.P)) {
            PhotonNetwork.LeaveRoom();  // 먼저 룸에서 나감
        }
    }

    // 룸을 나갔을 때 호출되는 콜백 함수
    public override void OnLeftRoom() {
        PhotonNetwork.Disconnect();  // Photon 서버 연결 끊기
    }

    // 서버 연결이 끊어졌을 때 호출되는 콜백 함수
    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause) {
        SceneManager.LoadScene("GameStart");  // 메인 씬으로 이동
    }
}
