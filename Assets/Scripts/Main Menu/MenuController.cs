using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuController : MonoBehaviour
{
    [Header("Host Game")]
    public string _newGameLevel;
    private string _levelToLoad;
    [SerializeField] private GameObject _hostGameDialog;

    public void HostGameDialogYes()
    {
        SceneManager.LoadScene(_newGameLevel);
    }   
}
