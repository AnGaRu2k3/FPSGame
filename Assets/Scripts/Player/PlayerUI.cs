using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using System;
using Photon.Pun;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Weapon weapon;

    [SerializeField] private Camera miniMap;
    [SerializeField] private GameObject playerOnMap;
    [SerializeField] private Slider healthBar;
    [SerializeField] private TMP_Text ammoStatus;
    [SerializeField] private TMP_Text healthStatus;
    [SerializeField] private TMP_Text timeRemainingStatus;


    [SerializeField] private GameObject deathScreen;
    [SerializeField] private Button respawnBtn;
    [SerializeField] private GameObject tabKDA;
    [SerializeField] private GameObject pauseFrame;


    public static PlayerUI Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        GameManager.Instance.OnTimeStatusUpdated += UpdateTimeRemainingStatus;
    }

    // Update is called once per frame
    void Update()
    {
        if (!player || !weapon) return;
        PlayerStatus playerStatus = player.GetComponent<PlayerStatus>();
        ammoStatus.text = $"{weapon.GetAmmo()} / {weapon.GetMagAmmo()}";
        // minimap follow player position and rotation
        miniMap.transform.position = new Vector3(player.transform.position.x, 300, player.transform.position.z);
        miniMap.transform.rotation = Quaternion.Euler(90, player.transform.rotation.eulerAngles.y, player.transform.rotation.eulerAngles.z);
        playerOnMap.transform.position = new Vector3(player.transform.position.x, 200, player.transform.position.z);
        // y + 45 because the image sprite is 45' degreed
        playerOnMap.transform.rotation = Quaternion.Euler(90, player.transform.rotation.eulerAngles.y + 45, player.transform.rotation.eulerAngles.z);
        // Update health status
        
        healthBar.value = playerStatus.GetHealth();
        if (Input.GetKey(KeyCode.Tab)) // hold tab button
        {
            tabKDA.GetComponent<CanvasGroup>().alpha = 1f;
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            tabKDA.GetComponent<CanvasGroup>().alpha = 0;
        }
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePauseFrame(true);
 
            }
        }

    }
    public void TogglePauseFrame(bool status)
    {
        bool controllStatus = (!status & !player.GetComponent<PlayerStatus>().IsDeath()); // if pause frame not open and player not death
        player.GetComponent<PlayerControllerUI>().EnableControls(controllStatus); 
        pauseFrame.SetActive(status);
    }
    private void UpdateTimeRemainingStatus(string status)
    {
        timeRemainingStatus.text = status;
    }

    public void SetPlayer(GameObject _player)
    {
        if (!_player.GetPhotonView().IsMine) return; // if not the local player;
        player = _player;
        weapon = player.GetComponentInChildren<Weapon>();
        respawnBtn.onClick.AddListener(() => player.GetComponent<PlayerStatus>().HandlePlayerRespawn());
    }

    public void UpdateHealth(int health)
    {
        healthStatus.text = health.ToString();
    }
    public void ToggleDeathScreen(bool status)
    {
        deathScreen.SetActive(status);

    }
    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnTimeStatusUpdated -= UpdateTimeRemainingStatus;
    }

}
