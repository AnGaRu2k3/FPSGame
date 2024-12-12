using System;
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

            await LoadGameSceneAsync();



            // Set up relay and start host
            await SetupRelayAndStartHost();
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

          
            await LoadGameSceneAsync();

            // Join relay
            await SetupRelayAndStartClient(lobby.LobbyCode);
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

            //await Task.Delay(2000);
            // Start host
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

    public void Cleanup()
    {
        // Add any cleanup logic here if needed
    }
}
