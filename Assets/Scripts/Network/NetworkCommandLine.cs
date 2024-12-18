using System;
using System.Collections;
using System.Collections.Generic;
using IngameDebugConsole;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetWorkCommandLine : MonoBehaviour
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

    private void Start()
    {       
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Connecting to Photon server...");
    }
    [ConsoleMethod("create-room", "create room with name and maxplayer")]
    public static void CreateRoom(string roomName, int maxPlayers)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)maxPlayers;
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;

        // Create room
        PhotonNetwork.CreateRoom(roomName, roomOptions);
        Debug.Log($"Room {roomName} created with max players: {maxPlayers}");

        
    }
    [ConsoleMethod("leave-room", "leave room")]
    public static void LeaveRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Home"); 
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







}
