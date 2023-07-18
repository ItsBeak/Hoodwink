using UnityEngine;
using Mirror;
using Cinemachine;
using UnityEngine.ProBuilder.Shapes;

public class H_PlayerBrain : NetworkBehaviour
{
    [Header("Player States & Settings")]
    public bool canMove = true;
    public bool canSprint;
    public bool canJump;
    [HideInInspector] public bool isPaused;
    public float speedMultiplier = 1;

    [Header("Player Colours")]
    [SyncVar(hook = nameof(SetShirtColour))] public Color shirtColour;
    [SyncVar(hook = nameof(SetPantsColour))] public Color pantsColour;
    [SyncVar(hook = nameof(SetShoesColour))] public Color shoesColour;

    [Header("Components")]
    public GameObject playerUI;
    public CinemachineVirtualCamera cam;
    public GameObject[] hideForLocalPlayer;
    public Renderer playerRenderer;

    private H_NetworkManager netManager;

    private H_NetworkManager NetManager
    {
        get
        {
            if (netManager != null) { return netManager; }
            return netManager = NetworkManager.singleton as H_NetworkManager;
        }
    }
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

            H_GameManager.instance.CmdRegisterPlayer(this);

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

    [Command]
    public void CmdSetPlayerColours()
    {
        shirtColour = H_GameManager.instance.shirtColours[Random.Range(0, H_GameManager.instance.shirtColours.Length)];
        pantsColour = H_GameManager.instance.shirtColours[Random.Range(0, H_GameManager.instance.shirtColours.Length)];
        shoesColour = H_GameManager.instance.shirtColours[Random.Range(0, H_GameManager.instance.shirtColours.Length)];
    }

    public void SetShirtColour(Color oldColor, Color newColor)
    {
        playerRenderer.material.SetColor("_ShirtColour", newColor);
    }

    public void SetPantsColour(Color oldColor, Color newColor)
    {
        playerRenderer.material.SetColor("_PantsColour", newColor);
    }

    public void SetShoesColour(Color oldColor, Color newColor)
    {
        playerRenderer.material.SetColor("_ShoesColour", newColor);
    }

    public void UnregisterPlayer()
    {
        H_GameManager.instance.CmdUnregisterPlayer(this);
    }
}
