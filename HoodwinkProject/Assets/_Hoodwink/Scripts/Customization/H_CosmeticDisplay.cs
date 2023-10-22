using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_CosmeticDisplay : MonoBehaviour
{
    public Transform hatAnchor;
    int hatIndex;
    int suitIndex;
    int vestIndex;

    public CosmeticSet[] suits, vests;

    [Header("Main Renderers")]
    public Renderer playerRenderer;
    public Renderer jacketRenderer, pantsRenderer;

    [Header("Secondary Renderers")]
    public Renderer vestRenderer;
    public Renderer[] collarRenderers;
    public Renderer[] tieRenderers;
    public Renderer[] pocketRenderers;
    public Renderer[] undershirtRenderers;

    [Header("Settings")]
    public bool usePlayerPrefs = true;

    private void Start()
    {
        if (usePlayerPrefs)
        {
            RefreshCosmetics();
        }
    }

    public void RefreshCosmetics()
    {
        hatIndex = PlayerPrefs.GetInt("C_SELECTED_HAT", 0);
        suitIndex = PlayerPrefs.GetInt("C_SELECTED_SUIT", 0);
        vestIndex = PlayerPrefs.GetInt("C_SELECTED_VEST", 0);

        ClearHat();

        SetHat(hatIndex);

        ToggleSuit(suitIndex);
        ToggleVest(vestIndex);
    }

    public void SetHat(int index)
    {
        Instantiate(H_CosmeticManager.instance.hats[index].cosmeticPrefab, hatAnchor);
    }

    public void ClearHat()
    {
        foreach (Transform child in hatAnchor.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void ToggleSuit(int index)
    {
        for (int i = 0; i < suits.Length; i++)
        {
            if (i == index)
            {
                foreach (GameObject part in suits[i].cosmeticParts)
                {
                    part.SetActive(true);
                }
            }
            else
            {
                foreach (GameObject part in suits[i].cosmeticParts)
                {
                    part.SetActive(false);
                }
            }
        }
    }

    public void ToggleVest(int index)
    {
        for (int i = 0; i < vests.Length; i++)
        {
            if (i == index)
            {
                foreach (GameObject part in vests[i].cosmeticParts)
                {
                    part.SetActive(true);
                }
            }
            else
            {
                foreach (GameObject part in vests[i].cosmeticParts)
                {
                    part.SetActive(false);
                }
            }
        }
    }

    public void SetColour(ColourData colourData)
    {
        SetJacketColour(colourData.primaryColour);
        SetPantsColour(colourData.pantsColour);
        SetVestColour(colourData.vestColour);
        SetTieColour(colourData.primaryColour);
        SetCollarColour(colourData.secondaryColour);
        SetPocketColour(colourData.secondaryColour);
    }

    public void SetJacketColour(Color newColor)
    {
        jacketRenderer.material.color = newColor;
    }

    public void SetPantsColour(Color newColor)
    {
        pantsRenderer.material.color = newColor;
    }

    public void SetVestColour(Color newColor)
    {
        vestRenderer.material.color = newColor;
    }

    public void SetCollarColour(Color newColor)
    {
        foreach (var collar in collarRenderers)
        {
            collar.material.color = newColor;
        }
    }

    public void SetTieColour(Color newColor)
    {
        foreach (var tie in tieRenderers)
        {
            tie.material.color = newColor;
        }
    }

    public void SetPocketColour(Color newColor)
    {
        foreach (var pocket in pocketRenderers)
        {
            pocket.material.color = newColor;
        }
    }

    public void SetUndershirtColour(Color newColor)
    {
        foreach (var undershirt in undershirtRenderers)
        {
            undershirt.material.color = newColor;
        }
    }
}
