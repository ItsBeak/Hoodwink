using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class H_GadgetBandage : H_GadgetBase
{

    [Header("Smoke Settings")]
    [SerializeField] int healAmount;
    H_PlayerHealth _playerHealth;
    private void Start()
    {
        _playerHealth = FindObjectOfType<H_PlayerHealth>();
    }

    public override void UseGadget()
    {
        Debug.Log("Heal");
        _playerHealth.Heal(healAmount);
    }
}
