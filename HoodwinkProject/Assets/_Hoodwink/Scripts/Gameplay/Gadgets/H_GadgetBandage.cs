using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Rendering;
using UnityEngine;

public class H_GadgetBandage : H_GadgetBase
{

    [Header("Bandage Settings")]
    [SerializeField] int healAmount;
    H_PlayerHealth playerHealth;

    public override void CmdUseGadget()
    {
        base.CmdUseGadget();

        playerHealth = GetComponentInParent<H_PlayerHealth>();

        Debug.Log("Heal");
        playerHealth.Heal(healAmount);
    }
}
