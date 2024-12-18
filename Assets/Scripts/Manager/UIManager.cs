using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class UIManager : MonoBehaviourPunCallbacks
{
    [Header("UI Buttons")]
    public Button hostButton;
    public Button joinButton;
    public Button quickJoinButton;

    [Header("UI Input Fields")]
    public TMP_InputField joinInputField;       
    public TMP_InputField usernameInputField;   

    [Header("UI Layouts")]
    public Canvas layout1;  // layout inputName
    public Canvas layout2;  // Layout choose room

    private void Start()
    {
        // Gắn sự kiện cho các button và input
        hostButton.onClick.AddListener(HostRoom);
        joinButton.onClick.AddListener(() => JoinRoom(joinInputField.text));
        quickJoinButton.onClick.AddListener(QuickJoinRoom);
        usernameInputField.onSubmit.AddListener(SubmitName);

        // Kết nối Photon khi bắt đầu
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("Connecting to Photon...");
        }
    }

    private void SubmitName(string playerName)
    {
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.Log("Player name cannot be empty.");
            return;
        }

        PhotonNetwork.NickName = playerName;
        Debug.Log($"Player name set to: {playerName}");

        layout1.gameObject.SetActive(false);
        layout2.gameObject.SetActive(true); 
    }

    //  Host Room
    private void HostRoom()
    {
        string roomName = "Room_" + UnityEngine.Random.Range(1000, 9999); // Tạo tên phòng ngẫu nhiên
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 10 };   // Giới hạn 10 người chơi
        PhotonNetwork.CreateRoom(roomName, roomOptions);
        Debug.Log($"Creating room: {roomName}");
    }

    // Tham gia phòng với mã phòng cụ thể
    private void JoinRoom(string roomCode)
    {
        if (string.IsNullOrEmpty(roomCode))
        {
            Debug.Log("Room code cannot be empty.");
            return;
        }

        PhotonNetwork.JoinRoom(roomCode);
        Debug.Log($"Joining room: {roomCode}");
    }

    // Tham gia phòng ngẫu nhiên
    private void QuickJoinRoom()
    {
        Debug.Log("Attempting to join a random room...");
        PhotonNetwork.JoinRandomRoom();
    }

    // ---------------- PHOTON CALLBACKS ----------------

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server.");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Successfully joined room: {PhotonNetwork.CurrentRoom.Name}");
        // Chuyển sang Scene Game nếu cần
        PhotonNetwork.LoadLevel("Game");
    }
}
