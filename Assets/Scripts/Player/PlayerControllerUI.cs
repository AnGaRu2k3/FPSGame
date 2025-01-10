using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Photon.Pun;
using Cinemachine;

public class PlayerControllerUI : MonoBehaviourPunCallbacks
{
    [SerializeField] private List<MonoBehaviour> playerScripts = new List<MonoBehaviour>();
    [SerializeField] private CinemachineFreeLook freeLook;
    [SerializeField] private GameObject menuFrame;
    
    void Start()
    {
        
        if (photonView.IsMine)
        {
            playerScripts.Add(gameObject.GetComponentInChildren<CharacterAiming>());

            freeLook = GameObject.Find("FreeLookCamera").GetComponent<CinemachineFreeLook>();
            EnableControls(true);
        }
        else
        {
            this.enabled = false;
        }
    }

    void Update()
    {

        
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
