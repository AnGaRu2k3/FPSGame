using UnityEngine;
using Photon.Pun;

public class PlayerSetUp : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject playerUI;
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
        player.GetComponentInChildren<Weapon>().SetLocalPlayer();
        playerUI.GetComponent<PlayerUI>().SetPlayer(player);
    }

}
