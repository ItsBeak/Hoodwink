using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SFXSlider : MonoBehaviour
{
    [SerializeField] private AudioMixer vol;
    private float SFXVolume = 1;
    private float sliderValue;
    [SerializeField] private Slider audioSlider;

    private void Awake()
    {
        LoadSettings();
    }
    public void SetLevel()
    {
        SFXVolume = audioSlider.value;
        sliderValue = SFXVolume;
        vol.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("sfxVolume", SFXVolume);
    }
    void LoadSettings()
    {
        SFXVolume = PlayerPrefs.GetFloat("sfxVolume", 1);
        audioSlider.value = SFXVolume;
    }
}

