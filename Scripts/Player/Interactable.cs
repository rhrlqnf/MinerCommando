using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��ȣ�ۿ� �������̽�
public interface IInteractable { 

    void ShowUI();
    void HideUI();
    void Interact(GameObject subject);
    GameObject gameObject { get; }  // GameObject �Ӽ� �߰�
}
