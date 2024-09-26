using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterSpawner : MonoBehaviourPun
{
    float lastSpawnTime;

    [SerializeField]
    float timeBetSpawn;

    [SerializeField]
    GameObject[] monsterPrefab;
    [SerializeField]
    GameObject spawnIndicator;
    bool spawnIndicatorActivated;
    [SerializeField]
    float maxCount;

    List<Monster> monsters = new();
    public void Spawn() {
        if (Time.time > lastSpawnTime + timeBetSpawn && !spawnIndicatorActivated && PhotonNetwork.IsMasterClient && monsters.Count<maxCount) {
  
            photonView.RPC("BlinkCoroutine", RpcTarget.All);

        }
        Debug.Log("spawn");
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, 1);
    }

    [PunRPC]
    private void BlinkCoroutine() {
        spawnIndicatorActivated = true;
        spawnIndicator.SetActive(true);
        StartCoroutine(Blink());
    }


    IEnumerator Blink() {
        int i = 5;
        bool toggle = false;
        while (i-- > 0) {

            spawnIndicator.SetActive(toggle = !toggle);
            Debug.Log(toggle);
            yield return new WaitForSeconds(0.4f);
        }
        spawnIndicatorActivated = false;
        lastSpawnTime = Time.time;

        if (PhotonNetwork.IsMasterClient) {
            Monster monster;
            if (new System.Random().NextDouble() < 1f / monsterPrefab.Length) {
                monster = PhotonNetwork.Instantiate(monsterPrefab[0].name, transform.position, new Quaternion()).GetComponent<Monster>();
            }
            else {
                monster = PhotonNetwork.Instantiate(monsterPrefab[1].name, transform.position, new Quaternion()).GetComponent<Monster>();
            }

            monsters.Add(monster);
            monster.onDeath += () => monsters.Remove(monster);
        }
        spawnIndicator.SetActive(false);
    }
}
