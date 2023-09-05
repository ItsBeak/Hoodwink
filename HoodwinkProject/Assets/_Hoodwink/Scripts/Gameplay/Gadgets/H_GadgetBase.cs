using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class H_GadgetBase : NetworkBehaviour
{
    [Header("Base Gadget Settings")]
    public string gadgetName;
    [TextArea] public string gadgetDescription;
    public Sprite gadgetIcon;
    public float cooldown = 5f;
    [HideInInspector] public float cooldownTimer = 0;
    [HideInInspector] public H_PlayerEquipment equipment;

    public virtual void Update()
    {
        if (!isOwned)
            return;

        if (cooldownTimer > 0)
        {
            cooldownTimer -= 1 * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && equipment.currentSlot == EquipmentSlot.Gadget)
        {
            UseGadgetPrimary();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && equipment.currentSlot == EquipmentSlot.Gadget)
        {
            UseGadgetSecondary();
        }
    }

    void ResetCooldown()
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

    public virtual void Initialize()
    {
        if (!equipment)
        {
            equipment = GetComponentInParent<H_PlayerEquipment>();
        }
    }

}
