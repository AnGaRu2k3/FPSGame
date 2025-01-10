using System.Collections;
using System.Collections.Generic;
using System.Security;
using TMPro;
using UnityEngine;


public class KillFeedTable : MonoBehaviour
{

    private List<KDA> playerKDAList = new List<KDA>();
    [SerializeField] private GameObject killFeedPrefab;
    [SerializeField] private Transform killFeedTable;
    static public KillFeedTable Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    public void createKillFeed(string killPlayerName, string deathPlayerName)
    {
        GameObject killFeed = Instantiate(killFeedPrefab, killFeedTable);
        TMP_Text[] texts = killFeed.GetComponentsInChildren<TMP_Text>();
        texts[0].text = killPlayerName;
        texts[1].text = deathPlayerName;
        Destroy(killFeed, 3f);
    }
}
