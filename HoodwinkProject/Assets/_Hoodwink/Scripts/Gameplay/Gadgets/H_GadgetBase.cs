using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class H_GadgetBase : NetworkBehaviour
{
    [Header("Base Gadget Settings")]
    public string gadgetName;
    public Sprite gadgetIcon;
    public float cooldown = 5f;
    [HideInInspector] public float cooldownTimer = 0;

    public virtual void Update()
    {
        if (!isOwned)
            return;

        if (cooldownTimer > 0)
        {
            cooldownTimer -= 1 * Time.deltaTime;
        }
    }

    public virtual void UseGadget()
    {
        if (cooldownTimer <= 0)
        {
            CmdUseGadget();
            cooldownTimer = cooldown;
        }
    }

    [Command(requiresAuthority = false)]
    public virtual void CmdUseGadget()
    {
        RpcUseGadget();
    }

    [ClientRpc]
    public virtual void RpcUseGadget()
    {
        Debug.Log(netIdentity + " used their gadget!");
    }

}
