using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class AudioManager : MonoBehaviour {
  private float volume = 1;

  [Header("Audio Sources")]
  [SerializeField] public AudioSource bgmAudioSource;
  [SerializeField] public AudioSource countdownAudioSource;
  [SerializeField] public AudioSource leaderboardAudioSource;
  

  [Header("Audio Clips")]
  [SerializeField] public AudioClip bgmClip;
  [SerializeField] public AudioClip countdownClip;
  [SerializeField] public AudioClip leaderboardClip;


  public static AudioManager instance { get;  set; }

  private void Update() {
    if (SceneManager.GetActiveScene().name == "MainMenu") {
      if (!instance.bgmAudioSource.isPlaying)  {
        Debug.Log("playing bg audio");

        instance.bgmAudioSource.Play();
      }
    } else {
      instance.bgmAudioSource.Stop();

      // Camera myCamera = Camera.main;
      // if (photonView.IsMine) {
      //   GameObject.FindGameObjectWithTag("Audio").transform.position = myCamera.transform.position;
      // }
    }

    instance.bgmAudioSource.volume = volume;
    instance.countdownAudioSource.volume = volume;
    instance.leaderboardAudioSource.volume = volume;

  }


  private void Awake() {
    if(instance != null && instance != this) {
      Destroy(gameObject);
    } else {
      instance = this;
      DontDestroyOnLoad(gameObject);
    }
  }

  public float GetVolume() {
    return volume;
  }

  private void Start() {
    instance.bgmAudioSource.clip = bgmClip;
    instance.bgmAudioSource.loop = true;
    instance.bgmAudioSource.Play();
  }

  public void PlayCountdown() {
    countdownAudioSource.PlayOneShot(countdownClip);
  }

  public void PlayLeaderboard() {
    leaderboardAudioSource.PlayOneShot(leaderboardClip);
  }

  public void SetVolume(float value) {
    volume = value / 100f;
    Debug.Log("Setting volume to " + value);
    // instance.bgmAudioSource.pitch = 0.1f;
  }
}