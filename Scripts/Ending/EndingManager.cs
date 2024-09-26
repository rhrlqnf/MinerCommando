using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class EndingManager : MonoBehaviourPun {


    public GameObject[] panels; // 패널들을 담는 배열
    public Text[] panelTexts; // 각 패널의 텍스트 오브젝트를 담는 배열
    public string[] dialogues; // 대사들을 담는 배열

    private int currentPanelIndex = 0;
    

    void Start() {
        Debug.Log("EndingManager Start called");
        Time.timeScale = 1;
        ShowPanel();
       
    }


    private void ShowPanel() {
        if (currentPanelIndex < panels.Length) {
            // 모든 패널을 비활성화
            foreach (var panel in panels) {
                panel.SetActive(false);
            }

            // 현재 패널을 활성화
            panels[currentPanelIndex].SetActive(true);


            if (currentPanelIndex < 3) {
                // TypewriterEffect 컴포넌트를 가져와서 이벤트를 등록
                TypewriterEffect2 typewriterEffect = panels[currentPanelIndex].GetComponentInChildren<TypewriterEffect2>();
                if (typewriterEffect != null) {
                    typewriterEffect.missionTextUI = panelTexts[currentPanelIndex];
                    typewriterEffect.missionDescription = dialogues[currentPanelIndex];
                    typewriterEffect.OnTypingComplete += OnTypingComplete;
                    typewriterEffect.StartTyping();
                }
            }
        }
        
    }

    private void OnTypingComplete() {

        StartCoroutine(WaitAndShowNextPanel());
    }
    private IEnumerator WaitAndShowNextPanel() {
        yield return new WaitForSeconds(2f); // 2초 대기
        currentPanelIndex++;
        ShowPanel();
    }

}
