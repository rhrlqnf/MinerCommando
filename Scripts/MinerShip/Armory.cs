using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//Ư�� ���ݿ� �ʿ��� ź���� �����ϰ� �ִ� �����
public class Armory : InteractableEntity {
    //��ȣ�ۿ��� �÷��̾��� �ڽ��� ź���� Ȱ��ȭ(�÷��̾ ź���� ��� �ִ� ��ó�� ���̰� ��)

    
   
    public override void Interact(GameObject subject) {
        base.Interact(subject);
        subject.transform.Find("SpecialAmmo").gameObject.SetActive(true);
    }
}
