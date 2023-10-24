using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DisplaySettings : MonoBehaviour
{

    private int fullscreen;
    [SerializeField] private Toggle fTog;
    [SerializeField] private TMP_Dropdown rDropdown;
    private int resolutionDropdown;


    private void Awake()
    {
        LoadSettings();
    }

    void ChangefTog()
    {
        if (fTog.isOn == false)
        {
            fullscreen = 0;
        }
        else if (fTog.isOn == true)
        {
            fullscreen = 1;
        }
    }
    void ChangeRes()
    {
        resolutionDropdown = rDropdown.value;
    }
    public void SaveSettings()
    {
        ChangefTog();
        ChangeRes();
        PlayerPrefs.SetInt("fullscreen", fullscreen);
        PlayerPrefs.SetInt("dropdown", resolutionDropdown);
        Debug.Log("save" + resolutionDropdown + " " + rDropdown.value);
    }
    void LoadSettings()
    {
        fullscreen = PlayerPrefs.GetInt("fullscreen", 1);
 
        resolutionDropdown = PlayerPrefs.GetInt("dropdown", 19);

        rDropdown.value = resolutionDropdown;
        Debug.Log("Load" + resolutionDropdown + " " + rDropdown.value);

        if (fullscreen == 0)
        {
            fTog.isOn = false;
        }
        if (fullscreen == 1)
        {
            fTog.isOn = true;
        }
    }
}
