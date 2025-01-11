using System;
using System.Collections;
using System.Collections.Generic;
using IngameDebugConsole;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetWorkCommandLine : MonoBehaviourPunCallbacks
{
    static public NetWorkCommandLine Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master server.");
        Debug.Log("Current region is: " + PhotonNetwork.CloudRegion);
    }

    private void Start()
    {       
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.ConnectToRegion("asia");
        Debug.Log("Connecting to Photon server...");
    }
    [ConsoleMethod("create-room", "create room with name")]
    public static void CreateRoom(string roomName)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 6;
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;

        // Create room
        PhotonNetwork.CreateRoom(roomName, roomOptions);
        Debug.Log($"Room {roomName} created with max players: 6)");

        
    }
    [ConsoleMethod("leave-room", "leave room")]
    public static void LeaveRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //PhotonNetwork.LoadLevel("Home"); 
            PhotonNetwork.LeaveRoom();  
            Debug.Log("Host has left the game.");
        }
        else if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();  
            Debug.Log("Client has left the game.");
        }
        else
        {
            Debug.Log("You are not in a room.");
        }
    }
    [ConsoleMethod("check-status", "Check current network status (host or client)")]
    public static void CheckStatus()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("You are the Host.");
            }
            else
            {
                Debug.Log("You are a Client.");
            }
        }
        else
        {
            Debug.Log("Not connected to Photon.");
        }
    }
    [ConsoleMethod("start-game", "Start the game (Only for Host)")]
    public static void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Starting game...");
            PhotonNetwork.LoadLevel("Game");  
        }
        else
        {
            Debug.Log("Only the Host can start the game.");
        }
    }
    [ConsoleMethod("join-room", "join room by name")]
    public static void JoinRoom(string roomName)
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Already in a room. Please leave the current room before joining another.");
            return;
        }

        PhotonNetwork.JoinRoom(roomName);
        Debug.Log($"join room: {roomName}");

    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        foreach(RoomInfo room in roomList)
        {
            Debug.Log("room name is: " + room.Name);
        }
    }
    [ConsoleMethod("quick-join", "Join a random room or create one if none exists")]
    public static void QuickJoin()
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Already in a room, leave before join r");
            return;
        }

        Debug.Log("Attempting to join a random room...");
        PhotonNetwork.JoinRandomRoom(); 
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No random room available, creating a new room...");
        string roomName = "Room_" + UnityEngine.Random.Range(1000, 9999); 
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 6, 
            IsOpen = true,
            IsVisible = true
        };

        PhotonNetwork.CreateRoom(roomName, roomOptions);
        Debug.Log($"Created a new room: {roomName}");
    }
    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined room: {PhotonNetwork.CurrentRoom.Name}");
    }

    [ConsoleMethod("set-player-name", "Set the player's name")]
    public static void SetPlayerName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.Log("Player name cannot be empty.");
            return;
        }
         
        PhotonNetwork.NickName = name; 
        Debug.Log($"Player name set to: {name}");
    }
    [ConsoleMethod("test-player-death", "Test the death view by character name")]
    public static void TestPlayerDeath(string gameObjectName)
    {
        GameObject player = GameObject.Find(gameObjectName);
        if (player != null && player.TryGetComponent<PlayerStatus>(out var playerStatus))
        {
            playerStatus.HandlePlayerDeath();
            Debug.Log($"Player {gameObjectName} death view triggered.");
        }
        else
        {
            Debug.Log($"GameObject {gameObjectName} not found or does not have a PlayerStatus component.");
        }
    }
    [ConsoleMethod("test-player-respawn", "Test the respawn view by character name")]
    public static void HandleRespawnPlayer(string gameObjectName)
    {
        GameObject player = GameObject.Find(gameObjectName);
        if (player != null && player.TryGetComponent<PlayerStatus>(out var playerStatus))
        {
            playerStatus.HandlePlayerRespawn();
            Debug.Log($"Player {gameObjectName} death view triggered.");
        }
        else
        {
            Debug.Log($"GameObject {gameObjectName} not found or does not have a PlayerStatus component.");
        }
    }







}
