using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class H_GadgetBase : NetworkBehaviour
{
    [Header("Base Gadget Settings")]
    public string gadgetName;
    [TextArea] public string gadgetDescription;
    public Sprite gadgetIcon;
    public float cooldown = 5f;
    [HideInInspector] public float cooldownTimer = 0;
    [HideInInspector] public H_PlayerEquipment equipment;

    [HideInInspector] public EquipmentSlot gadgetSlot;

    public GameObject gadgetVisuals;

    [Header("UI Components")]
    public bool usePrompt;
    public string prompt;
    public TextMeshProUGUI promptReadout;
    bool isInitialized;
    public virtual void Initialize()
    {
        if (!equipment)
        {
            equipment = GetComponentInParent<H_PlayerEquipment>();
        }

        HideGadget();

        isInitialized = true;
    }

    public virtual void Update()
    {
        if (!isOwned || !isInitialized)
            return;

        if (cooldownTimer > 0)
        {
            cooldownTimer -= 1 * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && equipment.currentSlot == gadgetSlot)
        {
            UseGadgetPrimary();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && equipment.currentSlot == gadgetSlot)
        {
            UseGadgetSecondary();
        }

        UpdateUI();
    }

    public void ResetCooldown()
    {
        cooldownTimer = cooldown;
    }

    public virtual void UseGadgetPrimary()
    {
        if (cooldownTimer <= 0)
        {
            CmdUseGadgetPrimary();
            ResetCooldown();
        }
    }

    public virtual void UseGadgetSecondary()
    {
        if (cooldownTimer <= 0)
        {
            CmdUseGadgetSecondary();
            ResetCooldown();
        }
    }

    [Command(requiresAuthority = false)]
    public virtual void CmdUseGadgetPrimary()
    {
        RpcUseGadgetPrimary();
    }

    [ClientRpc]
    public virtual void RpcUseGadgetPrimary()
    {

    }

    [Command(requiresAuthority = false)]
    public virtual void CmdUseGadgetSecondary()
    {
        RpcUseGadgetSecondary();
    }

    [ClientRpc]
    public virtual void RpcUseGadgetSecondary()
    {

    }

    public virtual void UpdateUI()
    {
        if (usePrompt)
        {
            if (cooldownTimer <= 0 && equipment.currentSlot == gadgetSlot)
            {
                promptReadout.text = prompt;

            }
            else
            {
                promptReadout.text = "";
            }
        }
        else
        {
            promptReadout.text = "";
        }
    }

    public void ShowGadget()
    {
        if (gadgetVisuals)
        {
            gadgetVisuals.SetActive(true);
        }
    }

    public void HideGadget()
    {
        if (gadgetVisuals)
        {
            gadgetVisuals.SetActive(false);
        }
    }
}
