using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Animations.Rigging;
using IngameDebugConsole;
using UnityEngine.Analytics;
using Cinemachine;

public class PlayerStatus : MonoBehaviourPun
{
    [SerializeField] private int id;
    [SerializeField] private int health;
    [SerializeField] private int kills = 0;
    [SerializeField] private int deaths = 0;
    [SerializeField] private string playerName;
    [SerializeField] private Weapon weapon;
    [SerializeField] private bool isDeath = false;
    [SerializeField] private int maxPriorityCinemachine = 10;
    
    public void SetUpWeapon(Weapon _weapon)
    {
        weapon = _weapon;
    }
    public void SetUp(int _id, string _playerName)
    {
        id = _id;
        playerName = _playerName;
    }
    public int GetHealth()
    {
        return health;
    }
    public void Update()
    {
        
    }
    public KDA GetKDA()
    {
        return new KDA(playerName, kills, deaths);
    }
    public void TakeDamage(int damage, GameObject shootingPlayer)
    {
        if (health <= 0) return; // do nothing  if get shoot after death
        health -= damage;
        
        if (health <= 0) 
        {
            health = 0;
            deaths++;
            isDeath = true;
            shootingPlayer.GetComponent<PlayerStatus>().kills++;
            PlayerStatusTableTab.Instance.DisplayTable();
            photonView.RPC("UpdateDeathAnimation", RpcTarget.All);
            if (photonView.IsMine) HandlePlayerViewDeath();

        }
        if (photonView.IsMine) PlayerUI.Instance.UpdateHealth(health);
    }
    public bool IsDeath()
    {
        return isDeath;
    }
    public void HandlePlayerViewDeath()
    {

        // player death view
        PlayerUI.Instance.ToggleDeathScreen(gameObject, true);
        // player controller
        gameObject.GetComponent<PlayerControllerUI>().EnableControls(false);

    }
    public void HandleRespawnPlayer()
    {
        // player controller
        gameObject.GetComponent<PlayerControllerUI>().EnableControls(true);
        // restore
        GameObject.Find("FreeLookCamera").GetComponent<CinemachineFreeLook>().Priority = MaxPriority();
        PlayerUI.Instance.ToggleDeathScreen(gameObject, false);
        Animator animator = gameObject.GetComponent<Animator>();
        gameObject.GetComponent<RigBuilder>().enabled = true;
        animator.SetTrigger("Respawn");
        // set weapon back to player
        Transform weaponAnchor = gameObject.transform.Find("WeaponAnchor").transform;
        weapon.transform.parent = weaponAnchor;
        weapon.transform.localPosition = weaponAnchor.Find("WeaponTransform").transform.localPosition;
        weapon.transform.rotation = weaponAnchor.Find("WeaponTransform").transform.rotation;
        // Destroy rb and collider
        Destroy(weapon.gameObject.GetComponent<Rigidbody>());
        Destroy(weapon.gameObject.GetComponent<MeshCollider>());
        // set height of character in idle
        gameObject.GetComponent<CharacterController>().height = 1.8f;
        gameObject.GetComponent<CapsuleCollider>().height = 1.8f;
        gameObject.GetComponent<CharacterController>().center = new Vector3(0, -0.2f, 0);
        gameObject.GetComponent<CapsuleCollider>().center = new Vector3(0, -0.2f, 0);
    }
    public int MaxPriority()
    {
        return ++maxPriorityCinemachine;
    }

    [PunRPC]
    public void UpdateDeathAnimation()
    {
        Animator animator = gameObject.GetComponent<Animator>();

        gameObject.GetComponent<RigBuilder>().enabled = false;
        animator.SetTrigger("Death");
        // set weapon out of player
        
        if (gameObject.GetComponentInChildren<Weapon>())
        {
            GameObject weapon = gameObject.GetComponentInChildren<Weapon>().gameObject;
            weapon.transform.parent = null;
            Rigidbody rb = weapon.AddComponent<Rigidbody>();
            MeshCollider collider = weapon.AddComponent<MeshCollider>();
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            collider.convex = true;
        }
        

        // set height of character when death
        gameObject.GetComponent<CharacterController>().height = 0.3f;
        gameObject.GetComponent<CharacterController>().center = Vector3.zero;
        gameObject.GetComponent<CapsuleCollider>().height = 0.3f;
        gameObject.GetComponent<CapsuleCollider>().center = Vector3.zero;
    }

}

