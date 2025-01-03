using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
public class PlayerStatus : MonoBehaviourPun
{
    [SerializeField] int id;
    [SerializeField] int health;
    [SerializeField] int kills;
    [SerializeField] int deaths;
    [SerializeField] Slider healthBar;
    public void SetID(int _id)
    {
        id = _id;
    }
    public int GetHealth()
    {
        return health;
    }

    [PunRPC]
    public void DealDamage(int damage)
    {
        health -= 30;
        healthBar.value -= damage;
        if (health <= 0)
        {
            // Todo: respawn player
        }
    }
}
