using UnityEngine;
using Mirror;
using Cinemachine;
using TMPro;
using UnityEngine.UI;

public class H_PlayerBrain : NetworkBehaviour
{
    [Header("Player States & Settings")]
    public bool canMove = true;
    public bool canSprint;
    public bool canJump;
    [HideInInspector] public bool isPaused;
    public float speedMultiplier = 1;

    [Header("Player Data")]
    [SyncVar(hook = nameof(OnNameChanged))] public string playerName;
    [SyncVar(hook = nameof(SetShirtColour))] public Color shirtColour;
    [SyncVar(hook = nameof(SetPantsColour))] public Color pantsColour;
    [SyncVar(hook = nameof(SetShoesColour))] public Color shoesColour;

    [Header("Components")]
    public GameObject playerUI;
    public CinemachineVirtualCamera cam;
    public Image agentColourImage;
    public TextMeshProUGUI agentNameText;
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

    public void SetShirtColour(Color oldColor, Color newColor)
    {
        playerRenderer.material.SetColor("_ShirtColour", newColor);
        agentColourImage.color = newColor;
    }

    public void SetPantsColour(Color oldColor, Color newColor)
    {
        playerRenderer.material.SetColor("_PantsColour", newColor);
    }

    public void SetShoesColour(Color oldColor, Color newColor)
    {
        playerRenderer.material.SetColor("_ShoesColour", newColor);
    }

    public void OnNameChanged(string oldName, string newName)
    {
        agentNameText.text = newName;
    }

    public void UnregisterPlayer()
    {
        H_GameManager.instance.CmdUnregisterPlayer(this);
    }
}


