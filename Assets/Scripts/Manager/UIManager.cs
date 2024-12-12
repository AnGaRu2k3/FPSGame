using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;

public class UIManager : MonoBehaviour
{
    public Button hostButton;
    public Button joinButton;
    public Button quickJoinButton;
    public TMP_InputField joinInputField;
    public TMP_InputField usernameInputField;

    public Canvas layout1;  // Canvas 1
    public Canvas layout2;  // Canvas 2

    private void Start()
    {
        hostButton.onClick.AddListener(HostLobby);
        joinButton.onClick.AddListener(() => JoinLobby(joinInputField.text));
        quickJoinButton.onClick.AddListener(QuickJoinLobby);
        usernameInputField.onSubmit.AddListener(SubmitName);
        joinInputField.onSubmit.AddListener(JoinLobby);
    }

    private async void SubmitName(string inputText)
    {
        // authentication
        try
        {
            InitializationOptions initOptions = new InitializationOptions();
            Debug.Log($"inpuText is {inputText}");
            initOptions.SetProfile(inputText);
           
            await UnityServices.InitializeAsync(initOptions);
           
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Successfully authenticated and initialized as {usernameInputField.text}");

            layout1.gameObject.SetActive(false);
            layout2.gameObject.SetActive(true);
        }
        catch (Exception e)
        {
            Debug.Log($"Initialization failed: {e.Message}");
        }
    }

    private void HostLobby()
    {
        Debug.Log("Hosting lobby...");
        LobbyAndRelayManager.Instance.HostLobbyAsync();
    }

    private void JoinLobby(string lobbyCode)
    {
        if (string.IsNullOrEmpty(lobbyCode))
        {
            Debug.Log("Please enter a valid lobby code.");
            return;
        }

        Debug.Log("Joining lobby with code: " + lobbyCode);
        LobbyAndRelayManager.Instance.JoinLobbyAsync(lobbyCode);
    }

    private void QuickJoinLobby()
    {
        Debug.Log("Quick joining a lobby...");
        // TODO: Implement random or automatic joining logic here
    }

    
}
