using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Photon.Pun;
using Cinemachine;

public class PlayerControllerUI : MonoBehaviourPunCallbacks
{
    [SerializeField] private List<MonoBehaviour> playerScripts = new List<MonoBehaviour>();
    [SerializeField] private CinemachineFreeLook freeLook;
    
    void Start()
    {
        playerScripts.Add(gameObject.GetComponentInChildren<CharacterAiming>());
        
        freeLook = GameObject.Find("FreeLookCamera").GetComponent<CinemachineFreeLook>();

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
            }
        }
        else if (Cursor.lockState == CursorLockMode.None)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && !IsPointerOverUI())
            {
                EnableControls(true);
            }
        }
    }

    public void EnableControls(bool status)
    {
        foreach (var script in playerScripts)
        {
            if (script != this)
            {
                script.enabled = status;
            }
        }
        Cursor.lockState = (status) ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !status;
        gameObject.GetComponentInChildren<CharacterMovement>().CanMove(status);
        // set camera 
        if (status)
        {
            freeLook.m_XAxis.m_MaxSpeed = 300f;
            freeLook.m_YAxis.m_MaxSpeed = 2f;
        }
        else
        {
            freeLook.m_XAxis.m_MaxSpeed = 0f;
            freeLook.m_YAxis.m_MaxSpeed = 0f;
        }
    }

    bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
