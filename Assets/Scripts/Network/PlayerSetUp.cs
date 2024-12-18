using UnityEngine;
using Photon.Pun;

public class PlayerSetUp : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    [Space]
    [SerializeField] Transform spawnPoint;
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }
    public void IsLocalPlayer()
    {
        
    }

}
