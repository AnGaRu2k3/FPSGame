using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button leaveroom;
    private void Start()
    {
        //leaveroom.onClick.AddListener(QuitRoom);
    }
    public void QuitRoom()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MainMenu");

    }
}
