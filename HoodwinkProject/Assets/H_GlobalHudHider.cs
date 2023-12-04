using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_GlobalHudHider : MonoBehaviour
{
    public CanvasGroup[] huds;

    bool localHide = false;
    public bool localOnly;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            localHide = !localHide;
        }

        foreach (var hud in huds)
        {
            if (localOnly)
            {
                hud.alpha = localHide ? 0 : 1;
            }
            else if (H_GameManager.instance)
            {
                if (localHide)
                {
                    hud.alpha = 0;
                }
                else
                {
                    hud.alpha = H_GameManager.instance.globalHideHud ? 0 : 1;
                }

            }
        }
    }
}
