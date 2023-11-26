using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_GlobalHudHider : MonoBehaviour
{
    public CanvasGroup[] huds;

    void Update()
    {
        foreach (var hud in huds)
        {
            if (H_GameManager.instance)
            {
                hud.alpha = H_GameManager.instance.globalHideHud ? 0 : 1;
            }
        }
    }
}
