using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Photon.Pun;

public class PlayerControllerUI : MonoBehaviourPunCallbacks
{
    [SerializeField] private List<MonoBehaviour> playerScripts = new List<MonoBehaviour>();
    [SerializeField] private Camera playerCamera;

    void Start()
    {
        GameObject player = transform.parent.gameObject;
        playerScripts.Add(player.GetComponentInChildren<PlayerMovement>());
        playerScripts.Add(player.GetComponentInChildren<Vision>());

        if (photonView.IsMine)
        {
            Camera.main.gameObject.SetActive(false);
            if (playerCamera != null)
            {
                playerCamera.gameObject.SetActive(true);
            }
            EnableControls(true);
        }
        else
        {
            playerCamera.gameObject.SetActive(false);
            EnableControls(false);
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                EnableControls(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && !IsPointerOverUI())
            {
                EnableControls(true);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    void EnableControls(bool status)
    {
        foreach (var script in playerScripts)
        {
            if (script != this)
            {
                script.enabled = status;
            }
        }
    }

    bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
