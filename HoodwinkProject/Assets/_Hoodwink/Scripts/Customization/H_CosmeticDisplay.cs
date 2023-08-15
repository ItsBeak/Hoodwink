using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_CosmeticDisplay : MonoBehaviour
{
    public Transform hatAnchor;
    int hatIndex;

    private void Start()
    {
        RefreshCosmetics();
    }

    public void RefreshCosmetics()
    {
        hatIndex = PlayerPrefs.GetInt("C_SELECTED_HAT", 0);

        foreach (Transform child in hatAnchor.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        Instantiate(H_CosmeticManager.instance.hats[hatIndex].cosmeticPrefab, hatAnchor);

    }
}
