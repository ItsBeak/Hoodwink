using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AmbienceVolume : MonoBehaviour
{
    [SerializeField] private AudioMixer vol;
    private float ambienceVolume = 1;
    private float sliderValue;
    [SerializeField] private Slider audioSlider;

    private void Awake()
    {
        LoadSettings();
    }
    public void SetLevel()
    {
        ambienceVolume = audioSlider.value;
        sliderValue = ambienceVolume;
        vol.SetFloat("AmbienceVolume", Mathf.Log10(sliderValue) * 20);
    }
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("AmbienceVolume", ambienceVolume);
    }
    void LoadSettings()
    {
        ambienceVolume = PlayerPrefs.GetFloat("AmbienceVolume", 1);
        audioSlider.value = ambienceVolume;
    }
}

