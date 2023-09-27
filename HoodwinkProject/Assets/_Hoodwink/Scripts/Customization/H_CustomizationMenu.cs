using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class H_CustomizationMenu : MonoBehaviour
{

    public H_CosmeticDisplay display;
    public TMP_InputField nameInput;

    private void Start()
    {
        nameInput.text = PlayerPrefs.GetString("C_SELECTED_NAME", "Hoodwinker");
    }

    public void SetHat(int index)
    {
        H_CosmeticManager.instance.SetHat(index);
        SaveAndRefresh();
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
