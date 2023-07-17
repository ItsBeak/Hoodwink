using UnityEngine;
using Mirror;
using Cinemachine;

public class H_PlayerBrain : NetworkBehaviour
{
    [Header("Player States & Settings")]
    public bool canMove = true;
    public bool canSprint;
    public bool canJump;
    [HideInInspector] public bool isPaused;
    public float speedMultiplier = 1;

    [Header("Components")]
    public GameObject playerUI;
    public CinemachineVirtualCamera cam;
    public GameObject[] hideForLocalPlayer;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isLocalPlayer)
        {
            cam.enabled = true;
            playerUI.SetActive(true);

            foreach (GameObject ob in hideForLocalPlayer)
            {
                ob.layer = LayerMask.NameToLayer("LocalPlayer");
            }

        }
    }

    public void SetCanMove(bool state)
    {
        canMove = state;
    }

    public void SetCanSprint(bool state)
    {
        canSprint = state;
    }

    public void SetCanJump(bool state)
    {
        canJump = state;
    }

    public void SetSpeedMultiplier(float amount)
    {
        speedMultiplier = amount;
    }
}
