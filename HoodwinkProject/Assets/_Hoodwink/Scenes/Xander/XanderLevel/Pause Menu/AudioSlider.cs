using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSlider : MonoBehaviour
{
    [SerializeField] private AudioMixer vol;

    public void SetLevel(float sliderValue)
    {
        vol.SetFloat("MasterVolume", Mathf.Log10(sliderValue) * 20);
    }
}

