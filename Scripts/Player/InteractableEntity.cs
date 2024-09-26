using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//��ȣ�ۿ� ������ ��ü

public class InteractableEntity : MonoBehaviourPun, IInteractable
{
    [SerializeField]
    private GameObject interactionUI;

    //���̵� UI Ȱ��ȭ
    //���̶���Ʈ�� Ȱ��ȭ�� ���� ����
    public void ShowUI() {
        interactionUI.SetActive(true);
        Debug.Log("show ui");
    }

    //���̵� UI ��Ȱ��ȭ
    //���̶���Ʈ ��Ȱ��ȭ�� ���� ����
    public void HideUI() {
        interactionUI.SetActive(false);
        Debug.Log("hide ui");
    }

    //��ȣ�ۿ�
    public virtual void Interact(GameObject subject) {
        Debug.Log("상호작용");
    }
}
