using UnityEngine;
using UnityEngine.Rendering;
using Mirror;
using SteamAudio;

public class H_PlayerCosmetics : MonoBehaviour
{
    public Transform hatAnchor;

    [Header("Main Renderers")]
    public Renderer playerRenderer;
    public Renderer jacketRenderer, pantsRenderer, socksRenderer, shoesRenderer;

    [Header("Secondary Renderers")]
    public Renderer[] tieRenderers;

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

    public void SetJacketColour(Color newColor)
    {
        //jacketRenderer.material.SetColor("_JacketColour", newColor);
        jacketRenderer.material.color = newColor;
    }

    public void SetPantsColour(Color newColor)
    {
        //pantsRenderer.material.SetColor("_PantsColour", newColor);
        pantsRenderer.material.color = newColor;
    }

    public void SetTieColour(Color newColor)
    {
        foreach (var tie in tieRenderers)
        {
            //tie.material.SetColor("_TieColour", newColor);
            tie.material.color = newColor;
        }
    }

}
