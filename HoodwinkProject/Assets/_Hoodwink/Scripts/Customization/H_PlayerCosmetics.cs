using UnityEngine;
using UnityEngine.Rendering;
using Mirror;
using SteamAudio;
using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class H_PlayerCosmetics : MonoBehaviour
{
    public Transform hatAnchor;
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
    public Renderer[] extraRenderers;

    public void SetHat(int hatIndex)
    {
        foreach (Transform child in hatAnchor.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        Instantiate(H_CosmeticManager.instance.hats[hatIndex].cosmeticPrefab, hatAnchor);

        if (GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            foreach (Renderer rend in hatAnchor.GetComponentsInChildren<Renderer>())
            {
                rend.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }
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

    public void HidePlayer()
    {
        playerRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        jacketRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        pantsRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        vestRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

        foreach (var collar in collarRenderers)
        {
            collar.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }

        foreach (var tie in tieRenderers)
        {
            tie.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }

        foreach (var pocket in pocketRenderers)
        {
            pocket.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }

        foreach (var undershirt in undershirtRenderers)
        {
            undershirt.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }

        foreach (var extra in extraRenderers)
        {
            extra.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }
    }

    public void ShowPlayer()
    {
        playerRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        jacketRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        pantsRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        vestRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

        foreach (var collar in collarRenderers)
        {
            collar.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }

        foreach (var tie in tieRenderers)
        {
            tie.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }

        foreach (var pocket in pocketRenderers)
        {
            pocket.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }

        foreach (var undershirt in undershirtRenderers)
        {
            undershirt.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }

        foreach (var extra in extraRenderers)
        {
            extra.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }

    }

}

[Serializable]
public class CosmeticSet
{
    public GameObject[] cosmeticParts;
}
