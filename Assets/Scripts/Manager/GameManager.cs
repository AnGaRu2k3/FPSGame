using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private float gameDuration = 300f;

    private float timeRemaining;
    private bool gameStarted = false;
    private bool countdownStarted = false;
    private float countdownTime = 3f; // Time for countdown before the game starts

    public static GameManager Instance { get; private set; }

    public event Action<string> OnTimeStatusUpdated; // Event to notify PlayerUI

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
            if (PhotonNetwork.PlayerList.Length >= 2)
            {
                StartCountdown();
            }
            else
            {
                NotifyTimeStatus("Waiting for other players");
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
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            NotifyTimeStatus();
        }
        else
        {
            EndGame();
        }
    }

    void EndGame()
    {
        gameStarted = false;
        NotifyTimeStatus("Game Over");
        Debug.Log("Game Over!");
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
}
