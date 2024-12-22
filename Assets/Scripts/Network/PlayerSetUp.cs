using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements;
public class PlayerSetUp : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject playerUI;
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
        int ID = player.GetComponent<PhotonView>().ViewID;
        player.GetComponent<PlayerStatus>().SetID(ID);
        //GameObject playerUI = GameObject.Find("PlayerUI");
        //if (playerUI == null)
        //{
        //    Debug.Log("Not found playerUI");
        //}
        //GameObject.
        //playerUI.GetComponent<PlayerUI>().SetHealthBar(player);
        player.GetComponentInChildren<Weapon>().SetLocalPlayer();
        
        playerUI.GetComponent<PlayerUI>().SetPlayer(player);
    }

}
