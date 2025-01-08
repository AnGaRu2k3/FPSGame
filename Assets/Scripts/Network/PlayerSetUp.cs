using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements;
using Cinemachine;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;
using UnityEngine.Animations;
using Unity.VisualScripting;

public class PlayerSetUp : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject playerUI;
    [SerializeField] CinemachineFreeLook freelookCamera;
    [SerializeField] CinemachineFreeLook aimFreeLookCamera;

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("A player has join room");
        // init player
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
        player.name = PhotonNetwork.NickName;
        // set up weapon
        Weapon weapon = player.GetComponentInChildren<Weapon>();
        if (weapon)
        {
            player.GetComponentInChildren<Weapon>().SetLocalPlayer();
            player.GetComponent<PlayerStatus>().SetUpWeapon(weapon);
        }
        // setup camera parameter
        int ID = player.GetComponent<PhotonView>().ViewID;
        player.GetComponent<PlayerStatus>().SetUp(ID, PhotonNetwork.NickName);

        if (player.GetPhotonView().IsMine)
        {
            // TPS camera
            freelookCamera.Follow = player.transform;
            foreach (Transform child in player.transform.GetComponentsInChildren<Transform>())
            {
                if (child.name == "mixamorig:Head")
                {
                    freelookCamera.LookAt = child;
                    break;
                }
            }
            aimFreeLookCamera = weapon.GetComponentInChildren<CinemachineFreeLook>();

        }
        // set up rig
        MultiAimConstraint[] aimConstraints = player.GetComponentsInChildren<MultiAimConstraint>();
        WeightedTransformArray sources = new WeightedTransformArray();
        sources.Add(new WeightedTransform(Camera.main.transform.Find("AimPoint"), 1.0f));
        foreach (MultiAimConstraint aimConstraint in aimConstraints)
            aimConstraint.data.sourceObjects = sources;
        player.GetComponent<Animator>().enabled = false;
        player.GetComponent<RigBuilder>().Build();
        player.GetComponent<Animator>().enabled = true;

        // update kda table
        PlayerStatusTableTab.Instance.DisplayTable();
        playerUI.GetComponent<PlayerUI>().SetPlayer(player);
    }

}
