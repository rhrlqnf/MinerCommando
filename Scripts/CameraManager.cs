using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance {
        get {
            if (m_instance == null) {
                m_instance = FindObjectOfType<CameraManager>();
            }

            return m_instance;
        }
    }

    private static CameraManager m_instance;
    private void Awake() {
        if (instance != this) {
            Destroy(gameObject);
        }
    }
    [SerializeField]
    CinemachineVirtualCamera cam;

    [SerializeField]
    GameObject minerShip;

    Coroutine zoom;

    public void GetInShip() {
        cam.Follow = minerShip.transform;
        if(zoom != null) {
            StopCoroutine(zoom);
        }
        zoom = StartCoroutine("Lerp", 11.25f);

    }

    public void GetOutShip(GameObject player) {
        cam.Follow = player.transform;
        if (zoom != null) {
            StopCoroutine(zoom);
        }
        zoom = StartCoroutine("Lerp", 8.71875f);
    }

    IEnumerator Lerp(float end) {
        while (cam.m_Lens.OrthographicSize != end) {
            cam.m_Lens.OrthographicSize = Mathf.Lerp(cam.m_Lens.OrthographicSize, end, 0.3f);

            yield return new WaitForSeconds(0.05f);
        }
    }
}
