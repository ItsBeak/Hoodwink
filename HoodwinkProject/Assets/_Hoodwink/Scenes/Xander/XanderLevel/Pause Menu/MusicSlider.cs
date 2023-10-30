using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MusicSlider : MonoBehaviour
{
    [SerializeField] private AudioMixer vol;
    private float musicVolume = 1;
    private float sliderValue;
    [SerializeField] private Slider audioSlider;

    private void Awake()
    {
        LoadSettings();
    }
    public void SetLevel()
    {
        musicVolume = audioSlider.value;
        sliderValue = musicVolume;
        vol.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
    }
    void LoadSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("musicVolume", 1);
        audioSlider.value = musicVolume;
    }
}

