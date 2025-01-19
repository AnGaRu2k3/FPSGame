using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class PlayerSFXManager : MonoBehaviourPun {
  private float volume = 1;

  [Header("Audio Sources")]
  [SerializeField] public AudioSource fireAudioSource;
  [SerializeField] public AudioSource reloadAudioSource;
  [SerializeField] public AudioSource walkAudioSource;
  [SerializeField] public AudioSource deathAudioSource;
  
  [Header("Audio Clips")]
  [SerializeField] public AudioClip fireClip;
  [SerializeField] public AudioClip reloadClip;
  [SerializeField] public AudioClip walkClip;
  [SerializeField] public AudioClip deathClip;

  public Animator animator;


  public void Start() {
    fireAudioSource = GameObject.Find("Fire").GetComponent<AudioSource>();
    reloadAudioSource = GameObject.Find("Reload").GetComponent<AudioSource>();
    walkAudioSource = GameObject.Find("Walk").GetComponent<AudioSource>();
    deathAudioSource = GameObject.Find("Death").GetComponent<AudioSource>();
    animator = GetComponent<Animator>();
  }

  public float GetVolume() {
    return volume;
  }

  public void PlayFire() {
    fireAudioSource.PlayOneShot(fireClip);
  }

  public  void PlayReload() {
    reloadAudioSource.PlayOneShot(reloadClip);
  }

  public void PlayWalk() {
    // GetComponent<PhotonView>().RPC("PlayWalk_RPC", RpcTarget.All);
    Debug.Log(animator.GetFloat("MovementX"));
    if(animator.GetFloat("MovementX") > 0.1f || animator.GetFloat("MovementZ") > 0.1f) {
        Debug.Log("Playing walk sound");
      if(!walkAudioSource.isPlaying) {
        walkAudioSource.PlayOneShot(walkClip);
      }
    }
  }

  public void PlayDeath() {
    deathAudioSource.PlayOneShot(deathClip);
  }

}