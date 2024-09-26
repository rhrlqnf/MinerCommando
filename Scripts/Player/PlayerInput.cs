using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//�÷��̾� ��ǲ

public class PlayerInput : MonoBehaviourPun {
    private string MoveXAxisName = "Horizontal";
    private string MoveYAxisName = "Vertical";
    private string InteractButtonName = "Interact";


    public float MoveX { get; private set;}
    public float MoveY { get; private set;}
    public bool Interact { get; private set;}

    //��ǲ�� ���� �� �ִ� ��������(ä������ ����, �̵��� �����ϰų� ���ڸ� ������϶� ��ǲ�� ���� �� ������)
    public bool canReceiveInput;

    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        //���ӿ����� ���� �ڵ� �ʿ� StageManager
        if (!canReceiveInput) {
            MoveX = 0;
            MoveY = 0;
            Interact = false;
            return;
        }

        MoveX= Input.GetAxisRaw(MoveXAxisName);
        MoveY = Input.GetAxisRaw(MoveYAxisName);

        Interact = Input.GetButtonDown(InteractButtonName);
    }
}

