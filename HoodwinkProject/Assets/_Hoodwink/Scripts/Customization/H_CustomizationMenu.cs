using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using System;

public class H_CustomizationMenu : MonoBehaviour
{

    public H_CosmeticDisplay display;
    public TMP_InputField nameInput;

    public ColourData[] colourPreviews; 

    private void Start()
    {
        nameInput.text = PlayerPrefs.GetString("C_SELECTED_NAME", "Hoodwinker");
    }

    public void SetHat(int index)
    {
        H_CosmeticManager.instance.SetHat(index);
        SaveAndRefresh();
    }

    public void SetSuit(int index)
    {
        H_CosmeticManager.instance.SetSuit(index);
        SaveAndRefresh();
    }

    public void SetVest(int index)
    {
        H_CosmeticManager.instance.SetVest(index);
        SaveAndRefresh();
    }

    public void PreviewColour(int index)
    {
        display.SetColour(colourPreviews[index]);
    }

    void SaveAndRefresh()
    {
        H_CosmeticManager.instance.SaveCosmetics();
        display.RefreshCosmetics();
    }

    public void SaveName()
    {
        PlayerPrefs.SetString("C_SELECTED_NAME", nameInput.text);

        if (nameInput.text == "")
        {
            PlayerPrefs.SetString("C_SELECTED_NAME", "Hoodwinker");
        }

        PlayerPrefs.Save();
    }
}

[Serializable]
public struct ColourData
{
    public Color primaryColour;
    public Color secondaryColour;
    public Color pantsColour, vestColour, tieColour;
}