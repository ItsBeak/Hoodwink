using Mirror;
using UnityEngine;

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


        if (!isLocalPlayer)
            return;

        playerAnimator.SetFloat("moveX", Input.GetAxis("Horizontal"));
        playerAnimator.SetFloat("moveY", Input.GetAxis("Vertical"));
    }
}
