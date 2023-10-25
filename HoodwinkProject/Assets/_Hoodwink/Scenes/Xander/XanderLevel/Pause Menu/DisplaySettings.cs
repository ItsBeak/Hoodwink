using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DisplaySettings : MonoBehaviour
{

    private int fullscreen;
    [SerializeField] private Toggle fTog;
    [SerializeField] private TMP_Dropdown quality;
    private int qualityAm;


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
    void changeQuality()
    {
        qualityAm = quality.value;
    }

    public void SaveSettings()
    {
        ChangefTog();
        changeQuality();
        PlayerPrefs.SetInt("fullscreen", fullscreen);
        PlayerPrefs.SetInt("quality", qualityAm);

    }
    void LoadSettings()
    {
        fullscreen = PlayerPrefs.GetInt("fullscreen", 1);
        qualityAm = PlayerPrefs.GetInt("quality", 2);

        quality.value = qualityAm;
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
