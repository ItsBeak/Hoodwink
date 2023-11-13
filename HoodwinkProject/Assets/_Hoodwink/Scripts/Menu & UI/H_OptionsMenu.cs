using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class H_OptionsMenu : MonoBehaviour
{
    public TMP_Dropdown qualityDropdown;

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index, true);
        Debug.Log("Set quality level to " + QualitySettings.GetQualityLevel());
    }

}
