using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyAndRelayManager : MonoBehaviour
{
    public static LobbyAndRelayManager Instance { get; private set; }

    [SerializeField] private string currentLobbyId;
    [SerializeField] private bool isLobbyActive;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async void HostLobbyAsync()
    {
        try
        {
            string lobbyCode = await CreateLobbyAsync();
            Debug.Log($"Lobby code is {lobbyCode}");
            // Set up relay and start host
            isLobbyActive = true;
            currentLobbyId = lobbyCode;
            _ = KeepLobbyAlive(currentLobbyId);
            await SetupRelayAndStartHost();
            // Move to gameScene
            await LoadGameSceneAsync();
        }
        catch (Exception e)
        {
            Debug.Log($"Failed to host lobby: {e.Message}");
        }
    }

    public async void JoinLobbyAsync(string lobbyCode)
    {
        try
        {
            var lobby = await LobbyService.Instance.GetLobbyAsync(lobbyCode);
            Debug.Log($"Joining lobby with code: {lobby.LobbyCode}");
            // Join relay
            await SetupRelayAndStartClient(lobby.LobbyCode);
            // Move to gameScene
            await LoadGameSceneAsync();
        }
        catch (Exception e)
        {
            Debug.Log($"Failed to join lobby: {e.Message}");
        }
    }

    private async Task<string> CreateLobbyAsync()
    {
        var lobbyOptions = new CreateLobbyOptions { IsPrivate = true };
        var lobby = await LobbyService.Instance.CreateLobbyAsync("MyLobby", 4, lobbyOptions);
        return lobby.LobbyCode;
    }

    private async Task SetupRelayAndStartHost()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4); // Max players = 4
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log($"Server relay code: {joinCode}");

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();

            Debug.Log($"Lobby created. Join code: {joinCode}");
        }
        catch (Exception e)
        {
            Debug.Log($"Failed to set up relay: {e.Message}");
        }
    }

    private async Task SetupRelayAndStartClient(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            //await Task.Delay(2000);
            NetworkManager.Singleton.StartClient();
            Debug.Log($"Successfully joined relay with join code: {joinCode}");
        }
        catch (Exception e)
        {
            Debug.Log($"Failed to join relay: {e.Message}");
        }
    }

    private async Task LoadGameSceneAsync()
    {
        if (SceneManager.GetActiveScene().name != "Game")
        {
            Debug.Log("Loading GameScene...");
            var loadOperation = SceneManager.LoadSceneAsync("Game");
            while (!loadOperation.isDone)
            {
                await Task.Yield(); 
            }
            Debug.Log("GameScene loaded.");
        }
    }
    private async Task KeepLobbyAlive(string lobbyId)
    {
        while (isLobbyActive)
        {
            try
            {
                await LobbyService.Instance.UpdateLobbyAsync(lobbyId, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        { "GameStarted", new DataObject(DataObject.VisibilityOptions.Public, "true") }
                    }
                });
                Debug.Log("Lobby status updated.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to update lobby: {e.Message}");
            }

            await Task.Delay(30000); // Update every 30 seconds
        }
    }

    public void Cleanup()
    {
        // Add any cleanup logic here if needed
    }
}
