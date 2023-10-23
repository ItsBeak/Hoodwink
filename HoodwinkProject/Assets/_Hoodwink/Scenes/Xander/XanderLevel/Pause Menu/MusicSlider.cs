using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicSlider : MonoBehaviour
{
    [SerializeField] private AudioMixer vol;

    public void SetLevel(float sliderValue)
    {
        vol.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
    }
}

