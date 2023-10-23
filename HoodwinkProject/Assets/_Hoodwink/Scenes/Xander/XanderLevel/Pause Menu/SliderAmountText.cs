using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderAmountText : MonoBehaviour
{
    [SerializeField] Slider _slider;
    [SerializeField] TMP_Text _amount;
    float temp;

    public void SliderAmountSet()
    {
        temp = _slider.value;
        temp = Mathf.Round(temp / 1.5f) * 1.5f;
        _amount.text = "" + temp * 100;
    }
}
