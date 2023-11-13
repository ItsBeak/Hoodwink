using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VoiceSlider : MonoBehaviour
{
    [SerializeField] private AudioMixer vol;
    private float voiceVolume = 1;
    private float sliderValue;
    [SerializeField] private Slider audioSlider;

    private void Awake()
    {
        LoadSettings();
    }
    public void SetLevel()
    {
        voiceVolume = audioSlider.value;
        sliderValue = voiceVolume;
        vol.SetFloat("VoiceVolume", Mathf.Log10(sliderValue) * 20);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("voiceVolume", voiceVolume);
    }
    void LoadSettings()
    {
        voiceVolume = PlayerPrefs.GetFloat("voiceVolume", 1);
        audioSlider.value = voiceVolume;
    }
}

