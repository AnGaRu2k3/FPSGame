using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    [SerializeField] private Button quickJoinBtn;
    [SerializeField] private Button hostRoomBtn;
    [SerializeField] private Button joinRoomBtn;
    [SerializeField] private TMP_InputField inputName;
    [SerializeField] private string roomName;
    [SerializeField] private TMP_InputField inputRoomName;

    private Action onNameSet; // either quickjoin or host or join room action
    void Start()
    {
        // set listerner
        inputName.onEndEdit.AddListener((name) => SetCharacterName(name));
        inputRoomName.onEndEdit.AddListener((name) => roomName = name);

        // set action
        quickJoinBtn.onClick.AddListener(() => onNameSet = QuickJoin);
        hostRoomBtn.onClick.AddListener(() => onNameSet = HostRoom);
        joinRoomBtn.onClick.AddListener(() => onNameSet = JoinRoom);

    }

    public void SetCharacterName(string name)
    {
        NetWorkCommandLine.SetPlayerName(name);
        SceneManager.LoadScene("Game");
        // either quick or join or host room action
        onNameSet?.Invoke();
    }

   

    private void QuickJoin()
    {
        NetWorkCommandLine.QuickJoin();

    }

    private void HostRoom()
    {
        NetWorkCommandLine.CreateRoom(roomName);
    }

    private void JoinRoom()
    {
        NetWorkCommandLine.JoinRoom(roomName);
    }
}
