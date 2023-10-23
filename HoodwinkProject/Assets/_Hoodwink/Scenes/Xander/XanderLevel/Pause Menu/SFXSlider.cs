using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFXSlider : MonoBehaviour
{
    [SerializeField] private AudioMixer vol;

    public void SetLevel(float sliderValue)
    {
        vol.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
    }
}

