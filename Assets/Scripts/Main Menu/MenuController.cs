using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class MenuController : MonoBehaviour
{
    [Header("Volume Setting")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private int defaultVolume = 40;

    [SerializeField] private GameObject comfirmationPrompt = null;

    [Header("Host Game")]
    public string _newGameLevel;
    private string _levelToLoad;
    [SerializeField] private GameObject _hostGameDialog;

    public void HostGameDialogYes()
    {
        SceneManager.LoadScene(_newGameLevel);
    }
    
    public void exitButton()
    {
        Application.Quit();
    }

    public void setVolume(float volume)
    {
       AudioListener.volume = volume;
       volumeTextValue.text = volume.ToString("0");
    }
    
    public void volumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ConfirmmationBox());
    }
    
    public void resetButton(string MenuType)
    {
        if (MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeTextValue.text = defaultVolume.ToString("0");
            volumeApply();
        }
    }    
    public IEnumerator ConfirmmationBox()
    {
        comfirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        comfirmationPrompt.SetActive(false);
    }    
}
