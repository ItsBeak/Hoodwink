using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_CosmeticManager : MonoBehaviour
{

    public static H_CosmeticManager instance { get; private set; }

    public Cosmetic[] hats;

    public AgentData[] agentColours;


    public Cosmetic currentHat;
    public int currentSuitCut, currentVestCut;

    void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        LoadCustomCharacter();
    }

    public void SaveCosmetics()
    {
        PlayerPrefs.SetInt("C_SELECTED_HAT", currentHat.ID);
        PlayerPrefs.SetInt("C_SELECTED_SUIT", currentSuitCut);
        PlayerPrefs.SetInt("C_SELECTED_VEST", currentVestCut);

        PlayerPrefs.Save();
    }

    public void LoadCustomCharacter()
    {
        currentHat = hats[PlayerPrefs.GetInt("C_SELECTED_HAT", 0)];
        currentSuitCut = PlayerPrefs.GetInt("C_SELECTED_SUIT", 0);
        currentVestCut = PlayerPrefs.GetInt("C_SELECTED_VEST", 0);

    }

    public void SetHat(int index)
    {
        currentHat = hats[index];
    }

    public void SetSuit(int index)
    {
        currentSuitCut = index;
    }

    public void SetVest(int index)
    {
        currentVestCut = index;
    }

}

[Serializable]
public class Cosmetic
{
    public int ID;
    public GameObject cosmeticPrefab;
}
