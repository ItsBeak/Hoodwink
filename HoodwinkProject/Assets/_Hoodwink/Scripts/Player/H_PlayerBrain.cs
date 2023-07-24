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
    [SyncVar] public bool hasAgentData;
    [SyncVar(hook = nameof(OnNameChanged))] public string playerName;
    [SyncVar(hook = nameof(SetShirtColour))] public Color shirtColour;
    [SyncVar(hook = nameof(SetPantsColour))] public Color pantsColour;
    [SyncVar(hook = nameof(SetShoesColour))] public Color shoesColour;
    [SyncVar(hook = nameof(OnReadyChanged))] public bool isReady = false;

    [Header("Alignment Data")]
    [SyncVar(hook = nameof(OnAlignmentChanged))]
    public AgentAlignment currentAlignment;
    public Color alignmentColorUnassigned, alignmentColorAgent, alignmentColorSpy;

    [Header("Components")]
    public GameObject playerUI;
    public CinemachineVirtualCamera cam;
    public Image agentColourImage;
    public TextMeshProUGUI agentNameText;
    public TextMeshProUGUI readyText;
    public TextMeshProUGUI alignmentText;
    public Image alignmentBackground;
    public H_PlayerEquipment equipment;

    [Header("Rendering")]
    public Renderer playerRenderer;
    public GameObject[] hideForLocalPlayer;

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

            HideLocalPlayer();

            foreach (GameObject ob in hideForLocalPlayer)
            {
                ob.layer = LayerMask.NameToLayer("LocalPlayer");
            }

            H_GameManager.instance.CmdRegisterPlayer(this);

            CmdSetReady(false);
            readyText.text = "Not Ready";
            readyText.color = Color.red;

            CmdSetAlignment(AgentAlignment.Unassigned);
        }
        else
        {
            playerUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            CmdSetReady(!isReady);
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

    [Command]
    void CmdSetAlignment(AgentAlignment alignment)
    {
        currentAlignment = alignment;
        UpdateAlignmentUI(currentAlignment);
        Debug.Log("Set role to: " + currentAlignment.ToString());
    }


    void OnAlignmentChanged(AgentAlignment oldRole, AgentAlignment newRole)
    {
        Debug.Log("Set changed to: " + newRole.ToString());

        UpdateAlignmentUI(newRole);
    }

    void UpdateAlignmentUI(AgentAlignment alignment)
    {
        alignmentText.text = currentAlignment.ToString();

        if (alignment == AgentAlignment.Unassigned)
        {
            alignmentBackground.color = alignmentColorUnassigned;
        }
        else if (alignment == AgentAlignment.Agent)
        {
            alignmentBackground.color = alignmentColorAgent;

        }
        else if (alignment == AgentAlignment.Spy)
        {
            alignmentBackground.color = alignmentColorSpy;
        }
    }

    [Command(requiresAuthority = false)]
    void CmdSetReady(bool ready)
    {
        isReady = ready;
    }

    void OnReadyChanged(bool oldReady, bool newReady)
    {
        if (newReady)
        {
            readyText.text = "Ready";
            readyText.color = Color.green;
        }
        else
        {
            readyText.text = "Not Ready";
            readyText.color = Color.red;
        }
    }

    [TargetRpc]
    public void TeleportPlayer(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        Physics.SyncTransforms();
    }

    public void HideLocalPlayer()
    {
        playerRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
    }

    public void ShowLocalPlayer()
    {
        playerRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }
}

public enum AgentAlignment
{
    Unassigned,
    Agent,
    Spy
}


