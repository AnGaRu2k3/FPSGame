using UnityEngine;
using UnityEngine.EventSystems;
using Unity.Netcode;
using Unity.VisualScripting;

public class PlayerControllerUI : NetworkBehaviour
{
    [SerializeField] private MonoBehaviour[] playerScripts;
    [SerializeField] private GameObject player;
    [SerializeField] private Camera playerCamera; 

    void Start()
    {
        //player = transform.parent.gameObject;
        //playerCamera = player.GetComponentInChildren<Camera>();
        playerScripts = player.GetComponentsInChildren<MonoBehaviour>();
        if (IsOwner)
        {
            Camera.main.gameObject.SetActive(false);
            playerCamera = Camera.main;

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
        if (!IsOwner) return;

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
            // If the script is not the PlayerControllerUI itself
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
