using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VoiceSlider : MonoBehaviour
{
    [SerializeField] private AudioMixer vol;

    public void SetLevel(float sliderValue)
    {
        vol.SetFloat("VoiceVolume", Mathf.Log10(sliderValue) * 20);
    }
}

