using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
public class MenuController : MonoBehaviour
{
    [Header("Volume Setting")]
    [SerializeField] private TMP_Text volumeTextMesh  = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private int volumeValue = 100;
    [SerializeField] private int defaultVolume = 100;

    [SerializeField] private GameObject comfirmationPrompt = null;

    [Header("Host Game")]
    public string _newGameLevel;
    private string _levelToLoad;
    [SerializeField] private GameObject _hostGameDialog;

    [Header("Graphics Setting")]
    [SerializeField] private Slider brightnessSlider = null;
    [SerializeField] private TMP_Text brightnessTextValue = null;
    [SerializeField] private int defaultBrightness = 50;

    private int _qualityLevel;
    private bool _fullscreen;
    private float _brightnessLevel;

    [Header("Resolution Dropdowns")]
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }   
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void setResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }    

    public void HostGameDialogYes()
    {
        SceneManager.LoadScene(_newGameLevel);
    }
    
    public void exitButton()
    {
        Application.Quit();
    }

    public void updateVolume()
    {
    //    AudioListener.volume = volume;
        Debug.Log("Volume: " + volumeSlider.value);
        volumeValue = (int)volumeSlider.value;
       volumeTextMesh.text = volumeValue.ToString();
    }
    
    public void volumeApply()
    {
        // PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        // StartCoroutine(ConfirmmationBox());
        AudioManager.instance.SetVolume(volumeValue);
    }
    
    public void resetButton(string MenuType)
    {
        if (MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeTextMesh.text = defaultVolume.ToString("0");
            volumeApply();
        }
    }    
    public IEnumerator ConfirmmationBox()
    {
        comfirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        comfirmationPrompt.SetActive(false);
    }
    
    public void setBrightness(float brightness)
    {
      _brightnessLevel = brightness;
       brightnessTextValue.text = brightness.ToString("0.0");
    }

    public void setQuality(int qualityIndex)
    {
        _qualityLevel = qualityIndex;
    }

    public void GraphicsApply()
    {
        PlayerPrefs.SetFloat("masterBrightness", _brightnessLevel);
        // Change your brightness with your post processing or whateber it is


        PlayerPrefs.SetInt("masterQuality", _qualityLevel);
        QualitySettings.SetQualityLevel(_qualityLevel);

        StartCoroutine(ConfirmmationBox());
    }
}
