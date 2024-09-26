using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableEntity : MonoBehaviourPun, IDamageable
{
    [SerializeField]
    private int _currentHp;
    public virtual int currentHp {
        get { return _currentHp; }
        set {
            _currentHp = value;
        }
    }

    [SerializeField]
    protected int maxHp;
    public bool isDead { get; protected set; }
    public event Action onDeath;
    public Transform centerPos;
    public float stopRad;

    protected virtual void OnEnable() {
        isDead = false;
        currentHp = maxHp;
    }

    [PunRPC]
    public virtual void OnDamage(int damage) {
        if (PhotonNetwork.IsMasterClient) {
            currentHp -= damage;

            photonView.RPC("ApplyUpdateHp", RpcTarget.Others, currentHp, isDead);

            photonView.RPC("OnDamage", RpcTarget.Others, damage);
        }
    }

    [PunRPC]
    public void ApplyUpdateHp(int hp, bool newDead) {
        currentHp = hp;
        isDead = newDead;
    }

    public virtual void Die() {
        if(onDeath != null) {
            onDeath();
        }
        isDead = true;
    }
}
