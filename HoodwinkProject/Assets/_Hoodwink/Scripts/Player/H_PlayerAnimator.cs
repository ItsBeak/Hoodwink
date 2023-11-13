using Mirror;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class H_PlayerAnimator : NetworkBehaviour
{
    private H_PlayerController playerController;
    private H_PlayerEquipment equipment;
    public Animator playerAnimator;
    public Animator fistsAnimator;

    void Start()
    {
        playerController = GetComponent<H_PlayerController>();
        equipment = GetComponent<H_PlayerEquipment>();
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        playerAnimator.SetBool("isSidearm", equipment.currentSlot == EquipmentSlot.Sidearm && equipment.sidearmClientObject);
        playerAnimator.SetBool("isItem", equipment.currentSlot == EquipmentSlot.PrimaryItem && equipment.primaryClientObject);

        if (playerController.isRunning)
        {
            playerAnimator.SetFloat("movementState", 1);
        }
        else
        {
            playerAnimator.SetFloat("movementState", 0f);
        }

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
