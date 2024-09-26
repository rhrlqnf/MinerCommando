using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Monster : DamageableEntity
{
    public LayerMask whatIsTarget;
    [SerializeField]
    protected DamageableEntity targetEntity;
    protected NavMeshAgent navMeshAgent;

    private Animator monsterAnim;

    protected Rigidbody2D rb;

    protected float basicSpeed;
    [SerializeField]
    float rushSpeed;
    [SerializeField]
    private float findTargetRad = 20f;
    [SerializeField]
    private int damage;
    [SerializeField]
    protected float timeBetAttack, timeBetKnockback;
    protected float lastAttackTime, knockbackTime;
    bool mbIsKnockbacked;

    [SerializeField]
    private bool mbShouldStop;
    protected DamageableEntity attackTarget;


    private bool hasTarget {
        get {
            if(targetEntity != null && !targetEntity.isDead) {
                return true;
            }

            return false;
        }
    }

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        monsterAnim = GetComponent<Animator>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        basicSpeed = navMeshAgent.speed;

        navMeshAgent.updatePosition = false;
    }

    private void Start() {
        if (PhotonNetwork.IsMasterClient) {
            StartCoroutine(FindTarget());
        }
    }

    private void Update() {
        if (!photonView.IsMine) {
            navMeshAgent.velocity = Vector3.zero;
            rb.velocity = Vector2.zero;
        }
        if (PhotonNetwork.IsMasterClient) {
            rb.velocity = navMeshAgent.desiredVelocity;
            navMeshAgent.nextPosition = transform.position;
            monsterAnim.SetBool("HasTarget", hasTarget);
            UpdateKnockback();
            Flip();
        }

        Debug.Log(navMeshAgent.isStopped);
    }

    //private void FixedUpdate() {
    //    if (PhotonNetwork.IsMasterClient) {
    //        rb.velocity = (navMeshAgent.nextPosition - transform.position).normalized * navMeshAgent.speed;
    //    }
    //}


    //마스터 클라에서만 실행(start에서)
    //이유:
    //transform과 animation을 포톤뷰로 연동받을 것이기 때문에 마스터가 아닌 클라이언트들에서는 몬스터가 직접 추적할 필요x
    //그냥 마스터 클라의 자기 분신을 따라하면 된다
    private IEnumerator FindTarget() {
        while (!isDead) {
            Collider2D[] colliders =
                        Physics2D.OverlapCircleAll(transform.position, findTargetRad, whatIsTarget);


            for (int i = 0; i < colliders.Length; i++) {
                DamageableEntity damageableEntity = colliders[i].GetComponent<DamageableEntity>();


                if (damageableEntity != null && !damageableEntity.isDead) {
                    if (targetEntity == null || targetEntity.isDead) {
                        targetEntity = damageableEntity;
                    }
                    else if (Vector2.Distance(damageableEntity.centerPos.position, transform.position) < Vector2.Distance(targetEntity.centerPos.position, transform.position))
                        targetEntity = damageableEntity;
                }
            }

            if (targetEntity != null) {
                navMeshAgent.SetDestination(
                            targetEntity.centerPos.position);
            }
            //navMeshAgent.stoppingDistance = targetEntity.stopRad;

            yield return new WaitForSeconds(0.25f);
        }
    }

    //마스터 클라에서만 동작되도록
    //모든 데미지 처리는 마스터에서 처리한 후 다른 클라들에 전달
    //짜피 데미지 처리가 마스터에서만 처리되도록 하면 알아서 마스터 클라에서만 동작됨
    [PunRPC]
    public override void OnDamage(int damage) {
        if (PhotonNetwork.IsMasterClient) {
            base.OnDamage(damage);

            navMeshAgent.isStopped = true;
            mbIsKnockbacked = true;
        }
        monsterAnim.SetTrigger("OnDamage");

        if (currentHp <= 0) {
            Die();
        }
        knockbackTime = Time.time;
    }

    public override void Die() {
        base.Die();
        navMeshAgent.isStopped = true;
        monsterAnim.SetBool("Die", true);
    }

    //오브젝트풀 만들 예정
    public void BackToPool() {
        //gameObject.SetActive(false);
        if (PhotonNetwork.IsMasterClient) {
            PhotonNetwork.Destroy(gameObject);
        }
    }


    //마스터에서만
    //이유:
    //데미지 처리는 마스터에서,
    //공격 애니메이션은 포톤뷰로 연동됨
    private void OnTriggerStay2D(Collider2D collision) {
        if (PhotonNetwork.IsMasterClient) {
            rb.velocity = Vector2.zero;

            if (!isDead && Time.time >= lastAttackTime + timeBetAttack) {
                attackTarget = collision.GetComponent<DamageableEntity>();
                if (attackTarget != null && attackTarget == targetEntity) {
                    lastAttackTime = Time.time;
                    //monsterAnim.SetTrigger("Attack");
                    photonView.RPC(nameof(SetAttackTrigger),RpcTarget.All);
                    if (mbShouldStop) {
                        navMeshAgent.isStopped = true;
                    }
                }
            }
        }
    }

    [PunRPC]
    public void SetAttackTrigger() {
        monsterAnim.SetTrigger("Attack");
    }

    //공격 애니메이션의 끝부분에 들어가는 함수
    //공격 애니메이션이 실행되고 일정 시간안에 범위를 벗어나면 맞지 않도록 구현하려고 분리함(아직은 구현x)
    public virtual void Attack() {
        if (PhotonNetwork.IsMasterClient) {
            attackTarget.OnDamage(damage);
            Debug.Log("damage:"+ damage);
        }
    }

    //맞으면 멈칫
    public void UpdateKnockback() {
        if (Time.time > knockbackTime + timeBetKnockback && !isDead && mbIsKnockbacked) {
            navMeshAgent.isStopped = false;
            mbIsKnockbacked = false;
        }

    }

    private void Flip() {
        if(!isDead && hasTarget) {
            if (transform.position.x > targetEntity.transform.position.x) {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    public void Stop() {
        navMeshAgent.isStopped = true;
    }

    public void Move() {
        navMeshAgent.isStopped = false;
    }

    public void Rush() {
        navMeshAgent.speed = rushSpeed;
    }

    public void ChangeSpeedToBasic() {
        navMeshAgent.speed = basicSpeed;
    }
}
