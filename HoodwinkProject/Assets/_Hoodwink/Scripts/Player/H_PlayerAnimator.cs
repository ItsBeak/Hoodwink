using Mirror;
using UnityEngine;

public class H_PlayerAnimator : NetworkBehaviour
{
    private H_PlayerController playerController;
    public Animator playerAnimator;

    void Start()
    {
        playerController = GetComponent<H_PlayerController>();
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        playerAnimator.SetFloat("moveX", Input.GetAxis("Horizontal"));
        playerAnimator.SetFloat("moveY", Input.GetAxis("Vertical"));
    }
}
