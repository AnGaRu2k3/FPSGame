using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour {
  private float volume = 1;

  [Header("Audio Sources")]
  [SerializeField] public AudioSource bgmAudioSource;
  [SerializeField] public AudioSource fireAudioSource;
  [SerializeField] public AudioSource reloadAudioSource;
  [SerializeField] public AudioSource walkAudioSource;
  [SerializeField] public AudioSource countdownAudioSource;
  [SerializeField] public AudioSource leaderboardAudioSource;
  [SerializeField] public AudioSource deathAudioSource;
  

  [Header("Audio Clips")]
  [SerializeField] public AudioClip bgmClip;
  [SerializeField] public AudioClip fireClip;
  [SerializeField] public AudioClip reloadClip;
  [SerializeField] public AudioClip walkClip;
  [SerializeField] public AudioClip countdownClip;
  [SerializeField] public AudioClip leaderboardClip;
  [SerializeField] public AudioClip deathClip;


  public static AudioManager instance { get;  set; }

  private void Update() {

    if (SceneManager.GetActiveScene().name == "test main") {
      if (!instance.bgmAudioSource.isPlaying)  {
        Debug.Log("playing audio");

        instance.bgmAudioSource.Play();
      }
    }

    instance.bgmAudioSource.volume = volume;
    instance.fireAudioSource.volume = volume;
    instance.reloadAudioSource.volume = volume;
    instance.walkAudioSource.volume = volume;
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

  public void PlayFire() {
    fireAudioSource.PlayOneShot(fireClip);
  }

  public void PlayReload() {
    reloadAudioSource.PlayOneShot(reloadClip);
  }

  public void PlayWalk() {
    Debug.Log("Playing walk sound");
    walkAudioSource.PlayOneShot(walkClip);
  }

  public void PlayCountdown() {
    countdownAudioSource.PlayOneShot(countdownClip);
  }

  public void PlayLeaderboard() {
    leaderboardAudioSource.PlayOneShot(leaderboardClip);
  }

  public void PlayDeath() {
    deathAudioSource.PlayOneShot(deathClip);
  }


  public void SetVolume(float value) {
    // AudioSource audioSource = GetComponent<AudioSource>();
    // audioSource.volume = value;
    // Debug.Log("Volume set to " + audioSource.volume);
    volume = value / 100f;
    Debug.Log("Setting volume to " + value);
    // instance.bgmAudioSource.pitch = 0.1f;
  }
}