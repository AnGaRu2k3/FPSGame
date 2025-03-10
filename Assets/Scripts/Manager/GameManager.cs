using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float gameDuration = 90f;

    private float timeRemaining;
    private bool gameStarted = false;
    private bool countdownStarted = false;
    private bool gameEnded = false;
    private float countdownTime = 3f; // Time for countdown before the game starts

    public static GameManager Instance { get; private set; }

    public event Action<string> OnTimeStatusUpdated; // Event to notify PlayerUI

    public GameObject ResultGameUI;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        timeRemaining = gameDuration;
        NotifyTimeStatus("Waiting for other players");
    }

    void Update()
    {
        if (!gameStarted && !countdownStarted)
        {
            int playerCount = GameObject.FindGameObjectsWithTag("Player").Length;
            if (playerCount >= 2)
            {
                StartCountdown();
            }
            else
            {
                NotifyTimeStatus("At least 2 players to start game");
            }
        }
        else if (countdownStarted && !gameStarted)
        {
            UpdateCountdown();
        }

        if (gameStarted)
        {
            UpdateTimer();
        }
    }

    void StartCountdown()
    {
        countdownStarted = true;
    }

    void UpdateCountdown()
    {
        if (countdownTime > 0)
        {
            countdownTime -= Time.deltaTime;
            int countdownSeconds = Mathf.CeilToInt(countdownTime);
            NotifyTimeStatus($"Game starts in {countdownSeconds}");
        }
        else
        {
            StartGame();
        }
    }

    void StartGame()
    {
        countdownStarted = false;
        gameStarted = true;
        timeRemaining = gameDuration;
        SpawnPlayers();
        NotifyTimeStatus();
    }   
    private void SpawnPlayers()
    {

    }
    void UpdateTimer()
    {
        if (gameEnded == true) return;
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            NotifyTimeStatus();
        }
        else
        {
            gameEnded = true;
            EndGame();
        }
    }

    void EndGame()
    {
        gameStarted = false;
        ShowResultGame();
        GameObject player = GlobalReferences.Instance.localPlayer;
        player.GetComponent<PlayerControllerUI>().EnableControls(false);
        PhotonNetwork.Destroy(player);
        
        ResultGameUI.SetActive(true);

    }
    public void Restart()
    {
        GameObject.FindObjectOfType<PlayerSetUp>().SetUpPlayer();
        ResultGameUI.SetActive(false);
        gameEnded = false;
        gameStarted = false;
        countdownStarted = false;
        timeRemaining = gameDuration;
        PlayerUI.Instance.ToggleDeathScreen(false);
    }


    public void ShowResultGame()
    {
        ResultGameUI.SetActive(true);
        AudioManager.instance.PlayLeaderboard();
    }
    private void NotifyTimeStatus(string customMessage = null)
    {
        if (customMessage != null)
        {
            OnTimeStatusUpdated?.Invoke(customMessage);
        }
        else
        {
            int minutes = ((int)timeRemaining) / 60;
            int seconds = ((int)timeRemaining) % 60;
            OnTimeStatusUpdated?.Invoke($"{minutes:D2}:{seconds:D2}");
        }
    }
    public bool ISGameStart()
    {
        return gameStarted;
    }
}
