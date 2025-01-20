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

public class PlayerStatus : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private int id;
    [SerializeField] private int health = 100;
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
        if (!photonView.IsMine) return; // only the player local run this kills
        if (health <= 0) return; // do nothing  if get shoot after death
        health -= damage;

        if (health <= 0)
        {
            health = 0;
            deaths++;
            isDeath = true;
            shootingPlayer.GetComponent<PlayerStatus>().kills++;
            string killerName = shootingPlayer.GetComponent<PlayerStatus>().playerName;
            shootingPlayer.GetComponent<PlayerStatus>().ApplyToSyncStatus();
            ApplyToSyncKillFeed(killerName, playerName);
            HandlePlayerDeath();

        }
        ApplyToSyncStatus();

        PlayerUI.Instance.UpdateHealth(health);
    }

    public bool IsDeath()
    {
        return isDeath;
    }

    public int MaxPriority()
    {
        return ++maxPriorityCinemachine;
    }
    [PunRPC]
    public void UpdateRespawnAnimation()
    {
        // restore
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

    public void HandlePlayerRespawn()
    {
        // update status
        health = 100;
        isDeath = false;
        ApplyToSyncStatus();
        // player death view
        PlayerUI.Instance.ToggleDeathScreen(false);
        // move Character to new spawnPoint
        Transform[] spawnPoints = GlobalReferences.Instance.spawnPoints;
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Debug.Log($"spawn point is {spawnPoint.position}");
        CharacterController controller = gameObject.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;
        gameObject.transform.position = spawnPoint.position;
        if (controller != null) controller.enabled = true;
        // player controller
        gameObject.GetComponent<PlayerControllerUI>().EnableControls(true);
        photonView.RPC("UpdateRespawnAnimation", RpcTarget.All);
        

        gameObject.transform.position = spawnPoint.position;
    }

    public void HandlePlayerDeath()
    {
        photonView.RPC("UpdateDeathAnimation", RpcTarget.All);
        // player death view
        PlayerUI.Instance.ToggleDeathScreen(true);
        // player controller
        gameObject.GetComponent<PlayerControllerUI>().EnableControls(false);
        gameObject.GetComponent<PlayerSFXManager>().PlayDeath();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
            stream.SendNext(kills);
            stream.SendNext(deaths);
            stream.SendNext(playerName);
            stream.SendNext(isDeath);
        }
        else if (stream.IsReading)
        {
            health = (int)stream.ReceiveNext();
            kills = (int)stream.ReceiveNext();
            deaths = (int)stream.ReceiveNext();
            playerName = (string)stream.ReceiveNext();
            isDeath = (bool)stream.ReceiveNext();

            PlayerStatusTableTab.Instance.DisplayTable();
        }
    }
    public void ApplyToSyncKillFeed(string killerName, string playerDeathName)
    {
        photonView.RPC("SyncKillFeed", RpcTarget.All, killerName, playerDeathName);
    }
    [PunRPC]
    public void SyncKillFeed(string killerName, string playerDeathName)
    {
        KillFeedTable.Instance.createKillFeed(killerName, playerDeathName);
    }
    public void ApplyToSyncStatus()
    {
        photonView.RPC("SyncPlayerStatus", RpcTarget.All, health, kills, deaths, playerName, isDeath);
    }
    [PunRPC]
    public void SyncPlayerStatus(int newHealth, int newKills, int newDeaths, string newPlayerName, bool newIsDeath)
    {

        health = newHealth;
        kills = newKills;
        deaths = newDeaths;
        playerName = newPlayerName;
        isDeath = newIsDeath;

        // updateUITableTab
        PlayerStatusTableTab.Instance.DisplayTable();
    }
}

