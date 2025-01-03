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

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("new player ");
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
        int ID = player.GetComponent<PhotonView>().ViewID;
        player.GetComponent<PlayerStatus>().SetID(ID);
        freelookCamera.Follow = player.transform;
        foreach (Transform child in player.transform.GetComponentsInChildren<Transform>())
        {
            if (child.name == "mixamorig:Head")
            {
                freelookCamera.LookAt = child;
                break;
            }
        }
        MultiAimConstraint[] aimConstraints = player.GetComponentsInChildren<MultiAimConstraint>();
        WeightedTransformArray sources = new WeightedTransformArray();
        sources.Add(new WeightedTransform(Camera.main.transform.Find("AimPoint"), 1.0f));
        foreach (MultiAimConstraint aimConstraint in aimConstraints)
            aimConstraint.data.sourceObjects = sources;
        player.GetComponent<Animator>().enabled = false;
        player.GetComponent<RigBuilder>().Build();
        player.GetComponent<Animator>().enabled = true;
        //player.transform.Find("CameraPos").position = Camera.main.transform.Find("AimPoint").position;
        //GameObject playerUI = GameObject.Find("PlayerUI");
        //if (playerUI == null)
        //{
        //    Debug.Log("Not found playerUI");
        //}
        //GameObject.
        //playerUI.GetComponent<PlayerUI>().SetHealthBar(player);
        if (player.GetComponentInChildren<Weapon>())
        player.GetComponentInChildren<Weapon>().SetLocalPlayer();
        
        
        playerUI.GetComponent<PlayerUI>().SetPlayer(player);
    }

}
