using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Tornado : MonoBehaviour
{
    GameObject minerShip;
    Rigidbody2D rb;

    [SerializeField]
    Transform[] pos;
    [SerializeField]
    LayerMask target;

    [SerializeField]
    float speed;
    private void Awake() {
        minerShip = FindAnyObjectByType<MinerShipHealth>().gameObject;
        rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        if (PhotonNetwork.IsMasterClient) {
            rb.velocity = (minerShip.transform.position - transform.position).normalized * speed;
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (PhotonNetwork.IsMasterClient && target == (target | (1 << other.gameObject.layer))) {
            Debug.Log("tornado");
            int index = new System.Random().Next(0, pos.Length);
            other.transform.position = pos[index].position;
        }
    }
    
}
