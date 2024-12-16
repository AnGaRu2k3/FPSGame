using System;
using System.Collections;
using System.Collections.Generic;
using IngameDebugConsole;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

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
    }

    [ConsoleMethod("start-host", "Start the server as Host")]
    public static void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("Host started");
    }

    [ConsoleMethod("start-client", "Start the client")]
    public static void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("Client started");
    }

    [ConsoleMethod("create-lobby", "Create a new lobby")]
    public static void CreateLobby(string playerName)
    {
        // todo with relay
        Debug.Log($"Creating lobby for player: {playerName}");
    }

    // Command to leave the host
    [ConsoleMethod("leave-host", "Stop the host and disconnect")]
    public static void LeaveHost()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.Shutdown(); 
            Debug.Log("Host has left the game");
        }
        else
        {
            Debug.Log("You are not the host, cannot leave as host.");
        }
    }

    // Command to leave the client
    [ConsoleMethod("leave-client", "Disconnect the client from the server")]
    public static void LeaveClient()
    {
        if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown(); 
            Debug.Log("Client has left the game");
        }
        else
        {
            Debug.Log("You are not connected as a client, cannot leave.");
        }
    }
}
