using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using TMPro;
public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Weapon weapon;

    [SerializeField] private Camera miniMap;
    [SerializeField] private GameObject playerOnMap;
    [SerializeField] private Slider healthBar;
    [SerializeField] private TMP_Text ammoStatus;
    [SerializeField] private TMP_Text healthStatus;

    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
        if (!player) return;
        PlayerStatus playerStatus = player.GetComponent<PlayerStatus>();
        ammoStatus.text = $"{weapon.GetAmmo()} / {weapon.GetMagAmmo()}";
        // minimap follow player position and rotation
        miniMap.transform.position = new Vector3(player.transform.position.x, 300, player.transform.position.z);
        miniMap.transform.rotation = Quaternion.Euler(90, player.transform.rotation.eulerAngles.y, player.transform.rotation.eulerAngles.z);
        playerOnMap.transform.position = new Vector3(player.transform.position.x, 200, player.transform.position.z);
        // y + 45 because the image sprite is 45' degreed
        playerOnMap.transform.rotation = Quaternion.Euler(90, player.transform.rotation.eulerAngles.y + 45, player.transform.rotation.eulerAngles.z);
        // Update health status
        healthStatus.text = playerStatus.GetHealth().ToString();
        healthBar.value = playerStatus.GetHealth();

        
    }
    //public void SetHealthBar(GameObject _healthBar)
    //{
    //    healthBar = _healthBar;
    //}
    //public void TakeDamage(int damage)
    //{
    //    healthBar.GetComponent<Slider>().value -= damage;
    //}
    private void SetWeapon(Weapon _weapon)
    {
        weapon = _weapon;
    }
    
    public void SetPlayer(GameObject _player)
    {
        player = _player;
        SetWeapon(player.GetComponentInChildren<Weapon>());
    }
        

}
