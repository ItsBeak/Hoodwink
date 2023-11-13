using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
    [SerializeField] private AudioMixer vol;
    private float masterVolume = 1;
    private float sliderValue;
    [SerializeField] private Slider audioSlider;

    private void Awake()
    {
        LoadSettings();
    }
    public void SetLevel()
    {
        masterVolume = audioSlider.value;
        sliderValue = masterVolume;
        vol.SetFloat("MasterVolume", Mathf.Log10(sliderValue) * 20);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("masterVolume", masterVolume);
    }
    void LoadSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("masterVolume", 1);
        audioSlider.value = masterVolume;
    }
}

