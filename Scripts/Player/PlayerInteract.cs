using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Photon.Pun;

//�÷��̾� ��ȣ�ۿ�
public class PlayerInteract : MonoBehaviourPun
{
    private PlayerInput playerInput;
    private Animator anim;
    private IInteractable interactionTarget;
    private Ore mineTarget;
    private void Start() {
        playerInput = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
    }

    private void Update() {
        if(!photonView.IsMine)
        {
            return;
        }


        Debug.Log("상호작용대상: " + interactionTarget);
        InteractWithObject();
        if (playerInput.MoveX != 0 || playerInput.MoveY != 0) {
            mineTarget = null;
        }
    }


    //Ʈ���� �ý������� ����� ��ü�� ��ȣ�ۿ�
    private void InteractWithObject()
    {
        if (interactionTarget != null && playerInput.Interact)
        {
            interactionTarget.Interact(gameObject);
        }
    }
   
    public void SwingPickaxe(Ore ore)
    {
        photonView.RPC("RPC_SwingPickaxe", RpcTarget.MasterClient, ore.gameObject.GetPhotonView().ViewID);
        anim.SetTrigger("SwingPickaxe");
        //mineTarget = ore;
    }
    //Mine
    [PunRPC]
    private void RPC_SwingPickaxe(int oreID)
    {
        PhotonView orePhotonView = PhotonView.Find(oreID);
        if (orePhotonView != null) {
            Ore ore = orePhotonView.GetComponent<Ore>();
            if (ore != null) {
                //anim.SetTrigger("SwingPickaxe");
                mineTarget = ore;
            }
        }
    }

    //Ÿ�� ������ ��̸� ���߽�Ų��
    //�ִϸ��̼��� ������ �����ӿ� ����Ǿ� �ִ�
    public void OnPickaxeStrike() {
        if (mineTarget != null) {
            mineTarget.OnHit();
        }
        else { Debug.Log("Strike Nothing"); }
    }



    //Trigger
    //��ȣ�ۿ��� ��ü����� ���� �ý���
    private void OnTriggerEnter2D(Collider2D collision) {
        if (photonView.IsMine) {
            IInteractable target = collision.GetComponent<IInteractable>();

            if (target != null) {
                if (interactionTarget != null) {
                    interactionTarget.HideUI();
                }
                interactionTarget = target;

                interactionTarget.ShowUI();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (photonView.IsMine) {
            IInteractable target = collision.GetComponent<IInteractable>();

            if (target != null) {
                if (target == interactionTarget) {
                    target.HideUI();
                    interactionTarget = null;
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (photonView.IsMine) {
            if (interactionTarget == null) {
                IInteractable target = collision.GetComponent<IInteractable>();
                if (target != null) {
                    interactionTarget = target;
                    interactionTarget.ShowUI();
                }
            }
        }
    }

}
