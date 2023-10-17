using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fullscreen : MonoBehaviour
{
    public Toggle fTog;

    public void Toggle()
    {
        if (fTog.isOn)
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
        if (fTog.isOn == false)
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }
}