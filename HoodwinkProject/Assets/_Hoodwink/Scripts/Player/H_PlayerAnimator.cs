using Mirror;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class H_PlayerAnimator : NetworkBehaviour
{
    private H_PlayerController playerController;
    private H_PlayerEquipment equipment;
    public Animator playerAnimator;

    void Start()
    {
        playerController = GetComponent<H_PlayerController>();
        equipment = GetComponent<H_PlayerEquipment>();
    }

    void Update()
    {
        playerAnimator.SetBool("isSidearm", equipment.currentSlot == EquipmentSlot.Sidearm && equipment.sidearmClientObject);

        if (playerController.isRunning)
        {
            playerAnimator.SetFloat("movementState", 1);
        }
        else if (false)
        {
            //playerAnimator.SetFloat("movementState", 0.0f);
        }
        else
        {
            playerAnimator.SetFloat("movementState", 0.5f);
        }

        if (!isLocalPlayer)
            return;

        playerAnimator.SetFloat("moveX", Input.GetAxis("Horizontal"));
        playerAnimator.SetFloat("moveY", Input.GetAxis("Vertical"));
    }

    [Command]
    public void CmdPlayPunchAnimation()
    {
        RpcPlayPunchAnimation();
    }

    [ClientRpc]
    public void RpcPlayPunchAnimation()
    {
        playerAnimator.SetTrigger("Punch");
    }

    [Command]
    public void CmdPlayStabAnimation()
    {
        RpcPlayStabAnimation();
    }

    [ClientRpc]
    public void RpcPlayStabAnimation()
    {
        playerAnimator.SetTrigger("Stab");
    }
}
