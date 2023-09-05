using Mirror;
using UnityEngine;
using UnityEngine.Rendering;

public class H_PlayerCosmetics : NetworkBehaviour
{
    public Transform hatAnchor;
    [SyncVar(hook = nameof(OnSetHat))] public int hatIndex;

    public override void OnStartLocalPlayer()
    {

        //int[] indexes = new int[12];

        int index;

        index = H_CosmeticManager.instance.currentHat.ID;

        CmdSetPlayerCosmetics(index);

        

    }

    [Command]
    void CmdSetPlayerCosmetics(int index)
    {
        hatIndex = index;
    }

    void OnSetHat(int oldHat, int newHat)
    {
        foreach (Transform child in hatAnchor.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        Instantiate(H_CosmeticManager.instance.hats[newHat].cosmeticPrefab, hatAnchor);

        if (isLocalPlayer)
        {
            foreach (Renderer rend in hatAnchor.GetComponentsInChildren<Renderer>())
            {
                rend.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }
        }
    }
}
