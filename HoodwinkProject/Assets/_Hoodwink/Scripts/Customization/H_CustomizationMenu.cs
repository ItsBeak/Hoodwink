using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_CustomizationMenu : MonoBehaviour
{

    public H_CosmeticDisplay display;

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
}
