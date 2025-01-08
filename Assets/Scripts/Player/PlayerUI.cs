using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using System;

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
    [SerializeField] private Button respawn;

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
    }

    private void UpdateTimeRemainingStatus(string status)
    {
        timeRemainingStatus.text = status;
    }

    public void SetPlayer(GameObject _player)
    {
        player = _player;
        weapon = player.GetComponentInChildren<Weapon>();
    }

    public void UpdateHealth(int health)
    {
        healthStatus.text = health.ToString();
    }
    public void ToggleDeathScreen(GameObject player, bool status)
    {
        deathScreen.SetActive(status);

    }
    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnTimeStatusUpdated -= UpdateTimeRemainingStatus;
    }

}
