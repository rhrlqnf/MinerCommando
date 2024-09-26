using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviourPun {
    public enum Sfx3D {
        normalAttack,
        specialAttack
    }
    public enum Sfx {
        loadAmmo,
        enter,
        item,
        hurt,
        engine,
        electricity,
        chest
    }

    public GameObject ship;

    public static SoundManager Instance { get; private set; }

    AudioSource bgmPlayer;
    AudioSource[] sfxPlayers;
    AudioSource[] sfx3DPlayers;
    [SerializeField]
    float bgmVolume, sfxVolume;
    [SerializeField]
    AudioClip[] bgmClip;
    [SerializeField]
    AudioClip[] sfxClip = { };
    [SerializeField]
    int channels;

    [SerializeField]
    AudioClip Bgm;

    private void Awake() {
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else {
            Destroy(this.gameObject );
        }
        

        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;


        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];


        for (int i = 0; i < sfxPlayers.Length; i++) {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].loop = false;
            sfxPlayers[i].volume = sfxVolume;
        }

        SceneManager.sceneLoaded += PlayBgm;
    }

    public void PlaySfx(Sfx sfx) {
        for (int i = 0; i < sfxPlayers.Length; i++) {
            if(!sfxPlayers[i].isPlaying){
                sfxPlayers[i].clip = sfxClip[(int)sfx];
                sfxPlayers[i].Play();
                break;
            }
        }

        //AudioSource.PlayClipAtPoint(sfxClip[(int)sfx], pos, sfxVolume);
    }

    public void PlaySfx3D(Sfx3D sfx) {
        sfx3DPlayers[(int)sfx].Play();
    }



    public void PlayBgm(Scene scene, LoadSceneMode mode) {
        ship = GameObject.Find("MinerShipSfx");
        if (ship != null)
            sfx3DPlayers = ship.GetComponents<AudioSource>();

        for (int i = 0; i < bgmClip.Length; i++) {
            if (bgmClip[i].name == SceneManager.GetActiveScene().name) {
                bgmPlayer.clip = bgmClip[i];
                break;
            }
        }
        bgmPlayer.Play();

    }
}
