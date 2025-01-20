using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System;

public class PlayerSFXManager : MonoBehaviourPun {
  private float volume = 1;

  [Header("Audio Sources")]
  [SerializeField] public AudioSource fireAudioSource;
  [SerializeField] public AudioSource reloadAudioSource;
  [SerializeField] public AudioSource walkAudioSource;
  [SerializeField] public AudioSource enemyWalkAudioSource;  
  [SerializeField] public AudioSource deathAudioSource;
  
  [Header("Audio Clips")]
  [SerializeField] public AudioClip fireClip;
  [SerializeField] public AudioClip reloadClip;
  [SerializeField] public AudioClip walkClip;
  [SerializeField] public AudioClip enemyWalkClip;
  [SerializeField] public AudioClip deathClip;

  public Animator animator;
  public bool isLocalPlayer = false;

  public void Start() {
    fireAudioSource = GameObject.Find("Fire").GetComponent<AudioSource>();
    reloadAudioSource = GameObject.Find("Reload").GetComponent<AudioSource>();
    walkAudioSource =  GameObject.Find("Walk").GetComponent<AudioSource>();
    enemyWalkAudioSource = GameObject.Find("EnemyWalk").GetComponent<AudioSource>();
    deathAudioSource = GameObject.Find("Death").GetComponent<AudioSource>();
    animator = GetComponent<Animator>();
    isLocalPlayer = gameObject.GetComponent<PhotonView>().IsMine;
  }

  public float GetVolume() {
    return volume;
  }

  public void PlayFire() {
    Debug.Log("Playing fire sound");
    fireAudioSource.PlayOneShot(fireClip);
  }

  public  void PlayReload() {
    reloadAudioSource.PlayOneShot(reloadClip);
  }

  public void PlayWalk() {
    Debug.Log(animator.GetFloat("MovementX"));
    if(Math.Abs(animator.GetFloat("MovementX")) > 0.5f || Math.Abs(animator.GetFloat("MovementZ")) > 0.5f) {
      Debug.Log("Playing walk sound");
      if(isLocalPlayer) {

        if(!walkAudioSource.isPlaying) {
          walkAudioSource.PlayOneShot(walkClip);
        } else {
          if(!enemyWalkAudioSource.isPlaying) {
            enemyWalkAudioSource.PlayOneShot(enemyWalkClip);
          }
        }
      }
    }
  }

  public void PlayDeath() {
    deathAudioSource.PlayOneShot(deathClip);
  }

}