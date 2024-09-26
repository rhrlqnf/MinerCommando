using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

public class PlayerHealth : DamageableEntity
{
    PlayerEquipment playerEquipment;

    public override int currentHp {
        get { return base.currentHp; }
        set {
            base.currentHp = value;
            if (photonView.IsMine) {
                UIManager.instance.UpdateHp(currentHp,playerEquipment.CurrentShield);
            }
        }
    }

    [SerializeField]
    float spawnRad;

    [SerializeField]
    LayerMask whatIsSpawner;

    float lastKnockbackTime;
    [SerializeField]
    float timeBetKnockback;

    //온도 시스템을 도입하면서 탐사선 내부에서 조종 중 죽는경우의 키입력 버그 때문에 주석처리
    PlayerInput playerInput;
    Animator animator;
    Inventory inventory;

    bool isKnockbacked;


    ShipController[] shipControllers;


    private void Awake() {
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        inventory = FindAnyObjectByType<Inventory>();
        playerEquipment = GetComponent<PlayerEquipment>();
    }

    private void Start() {
        shipControllers = FindObjectsByType<ShipController>(FindObjectsSortMode.None);
    }

    protected override void OnEnable() {
        base.OnEnable();
        if (PhotonNetwork.IsMasterClient) {
            InvokeRepeating("SpawnTrigger", 5, 0.2f);
        }
        isKnockbacked = false;
        photonView.RPC("LightOff", RpcTarget.All);

        playerInput.canReceiveInput = true;
    }

    private void Update() {
        UpdateKnockback();
    }

    [PunRPC]
    public override void OnDamage(int damage) {
        if (PhotonNetwork.IsMasterClient) {
            damage /= 2;
            if (playerEquipment.CurrentShield > 0) {
                int reducedDamage = damage - playerEquipment.CurrentShield;
                playerEquipment.CurrentShield-=damage;
                damage = reducedDamage > 0?reducedDamage:0;
            }
            base.OnDamage(damage);
        }
        if (currentHp <= 0) {
            Die();
        }
        Debug.Log("되나");
        lastKnockbackTime = Time.time;
        isKnockbacked = true;
        animator.SetTrigger("OnDamage");
        if (photonView.IsMine) {
            SoundManager.Instance.PlaySfx(SoundManager.Sfx.hurt);
            foreach (var controller in shipControllers) {
                controller.StopControl();
            }
        }
        playerInput.canReceiveInput = false;
    }

    public override void Die() {
        base.Die();
        isKnockbacked = true;
        animator.SetBool("Die", true);
        if (photonView.IsMine) {
            inventory.DropOre(transform.position);
        }
        if (PhotonNetwork.IsMasterClient) {
            CancelInvoke("SpawnTrigger");
        }
    }

    public void BackToPool() {
        gameObject.SetActive(false);
        if (photonView.IsMine) {
            CameraManager.instance.GetInShip();
        }
        Invoke("Respawn", 10);
    }

    public void Respawn() {
        gameObject.SetActive(true);
        Transform respawnPos = GameObject.Find("Respawn").transform;
        transform.position = respawnPos.position;
    }

    public void UpdateKnockback() {
        if (Time.time > lastKnockbackTime + timeBetKnockback && isKnockbacked &&!isDead) {
            playerInput.canReceiveInput = true;
            isKnockbacked = false;
        }

    }

    private void SpawnTrigger() {
        Collider2D[] spawners = Physics2D.OverlapCircleAll(transform.position, spawnRad, whatIsSpawner);
        Debug.Log("s");
        foreach (Collider2D collider in spawners) {
            MonsterSpawner spawner = collider.GetComponent<MonsterSpawner>();
            if (spawner != null) {
                spawner.Spawn();
                Debug.Log("spawnTrigger");
            }
        }

    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, spawnRad);
    }

}
