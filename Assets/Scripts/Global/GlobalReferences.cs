using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class GlobalReferences : MonoBehaviour
{
    public static GlobalReferences Instance{get; set;}
    public Transform[] spawnPoints;
    public GameObject bulletImpactPrefab;
    public GameObject bloodSprayPrefab;
    public string localPlayerName;
    public GameObject localPlayer;
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else Instance = this;
    }
}
