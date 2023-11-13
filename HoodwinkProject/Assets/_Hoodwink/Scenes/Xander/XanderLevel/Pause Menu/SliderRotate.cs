using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SliderRotate : MonoBehaviour
{
    [SerializeField] private GameObject handle;
    [SerializeField] private Slider slider;
    public void Rotate()
    {
        float sliderValue = slider.value;
        handle.transform.rotation = Quaternion.Euler(0, 0, sliderValue * -360);
    }
}
