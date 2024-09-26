using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterTypeA : Monster
{
    [SerializeField]
    CapsuleCollider2D col;
    public override void Attack() {
        if (PhotonNetwork.IsMasterClient) {

            var hits = new List<RaycastHit2D>();
            var targets = new HashSet<DamageableEntity>();

            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.layerMask = whatIsTarget.value;
            contactFilter.useLayerMask = true;
            col.Cast(Vector2.zero, contactFilter, hits, 0, true);
            foreach (var hit in hits) {
                Debug.Log(hit.transform.name);
                targets.Add(hit.transform.GetComponent<DamageableEntity>());
            }
            foreach (var target in targets) {
                attackTarget = target;
                base.Attack();
            }
        }
    }
}
