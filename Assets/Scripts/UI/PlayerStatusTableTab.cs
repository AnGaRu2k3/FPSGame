using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
public class KDA
{
    public string playerName;
    public int kills;
    public int deaths;

    public KDA(string _playerName, int _kills, int _deaths)
    {
     
        playerName = _playerName;
        kills = _kills;
        deaths = _deaths;
    }

    public float GetKDRatio()
    {
        return deaths == 0 ? kills : (float)kills / deaths;
    }
}

public class PlayerStatusTableTab : MonoBehaviour
{

    private List<KDA> playerKDAList = new List<KDA>();
    [SerializeField] private GameObject playerKDAPrefab;
    [SerializeField] private Transform playerListContainer;

    static public PlayerStatusTableTab Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        DisplayTable();
    }
    public void DisplayTable()
    {
        playerKDAList.Clear();

        PlayerStatus[] allPlayerStatuses = FindObjectsOfType<PlayerStatus>();


        foreach (PlayerStatus playerStatus in allPlayerStatuses)
        {
            KDA kDA = playerStatus.GetKDA();

            playerKDAList.Add(kDA);
        }

        // sort
        playerKDAList = playerKDAList.OrderByDescending(k => k.GetKDRatio()).ToList();
        // delete all old row
        foreach (Transform child in playerListContainer)
        {
            Destroy(child.gameObject);
        }
        // loop and create playerKDAUI
        foreach (var kda in playerKDAList)
        {
            GameObject item = Instantiate(playerKDAPrefab, playerListContainer);

            // Gán thông tin vào UI
            TMP_Text[] texts = item.GetComponentsInChildren<TMP_Text>();
            texts[0].text = kda.playerName; // Name
            texts[1].text = kda.kills.ToString(); // Kills
            texts[2].text = kda.deaths.ToString(); // Deaths
        }
    }
}

