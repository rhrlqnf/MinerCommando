using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;

//�÷��̾� �̵�

public class PlayerMovement : MonoBehaviourPun
{
    Rigidbody2D playerRb;
    PlayerInput playerInput;
    Animator anim;

    private bool isMoving;

    private bool facingRight = true;

    [SerializeField]
    private float moveSpeed;

    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        if(!photonView.IsMine)
        {
            playerRb.velocity = Vector3.zero;
            return;
        }

        if (playerInput.MoveX != 0 && (facingRight ^ playerInput.MoveX > 0)) {
            Flip();
        }

        Move();

        anim.SetBool("isMoving", isMoving);
    }
    
    private void Move() {
        playerRb.velocity = new Vector2(playerInput.MoveX, playerInput.MoveY) * moveSpeed;

        if (playerRb.velocity != Vector2.zero) {
            isMoving = true;
        }
        else {
            isMoving = false;
        }
    }

    private void Flip() {
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }
}


