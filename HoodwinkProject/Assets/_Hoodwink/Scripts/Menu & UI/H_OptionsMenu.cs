using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class H_OptionsMenu : MonoBehaviour
{
    public Dropdown qualityDropdown;

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index, true);
        Debug.Log("Set quality level to " + QualitySettings.GetQualityLevel());
    }

}
