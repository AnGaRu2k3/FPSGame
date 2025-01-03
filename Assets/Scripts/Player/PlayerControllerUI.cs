using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Photon.Pun;
using Cinemachine;

public class PlayerControllerUI : MonoBehaviourPunCallbacks
{
    [SerializeField] private List<MonoBehaviour> playerScripts = new List<MonoBehaviour>();
    [SerializeField] private GameObject freelookCamera;
    void Start()
    {
        freelookCamera = GameObject.Find("FreeLookCamera");
        playerScripts.Add(gameObject.GetComponentInChildren<CharacterAiming>());
        playerScripts.Add(gameObject.GetComponentInChildren<CharacterMovement>());
        playerScripts.Add(gameObject.GetComponentInChildren<Weapon>());

        if (photonView.IsMine)
        {
            EnableControls(true);
        }
        else
        {
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
        freelookCamera.SetActive(status);
    }

    bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
