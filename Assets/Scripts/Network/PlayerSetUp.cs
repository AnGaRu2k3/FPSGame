using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements;
using Cinemachine;
public class PlayerSetUp : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject playerUI;
    [SerializeField] CinemachineFreeLook freelookCamera;
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
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
