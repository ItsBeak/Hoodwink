using UnityEngine;
using Mirror;
using Cinemachine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;

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
    [SyncVar(hook = nameof(SetCoatColour))] public Color coatColour;
    [SyncVar(hook = nameof(SetCoatTrimColour))] public Color coatTrimColour;
    [SyncVar(hook = nameof(SetPantsColour))] public Color pantsColour;
    [SyncVar(hook = nameof(SetShoesColour))] public Color shoesColour;
    [SyncVar(hook = nameof(OnReadyChanged))] public bool isReady = false;

    [Header("Alignment Data")]
    [SyncVar(hook = nameof(OnAlignmentChanged))]
    public AgentAlignment currentAlignment;
    public Color alignmentColorUnassigned;
    public Color alignmentColorAgent;
    public Color alignmentColorSpy;

    [Header("Alignment Data")]
    public GameObject spyIndicator;
    public LayerMask baseCullingMask, spyCullingMask;

    [Header("Components")]
    public CinemachineVirtualCamera cam;
    public Image agentColourImage;
    public TextMeshProUGUI agentNameText;
    public TextMeshProUGUI readyText;
    public H_PlayerEquipment equipment;
    public H_PlayerUI playerUI;

    [Header("Rendering")]
    public Renderer playerRenderer;
    public Renderer coatRenderer, coatTrimRenderer;
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
            playerUI.gameObject.SetActive(true);

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
            playerUI.gameObject.SetActive(false);
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

    public void SetCoatColour(Color oldColor, Color newColor)
    {
        playerRenderer.material.SetColor("_ShirtColour", Color.clear);
        coatRenderer.material.color = newColor;
        agentColourImage.color = newColor;
    }

    public void SetCoatTrimColour(Color oldColor, Color newColor)
    {
        coatTrimRenderer.material.color = newColor;
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
        playerUI.alignmentText.text = currentAlignment.ToString();
        playerUI.alignmentFolderText.text = currentAlignment.ToString();
        spyIndicator.SetActive(false);

        if (alignment == AgentAlignment.Unassigned)
        {
            playerUI.alignmentBackground.color = alignmentColorUnassigned;
            playerUI.roleAnimator.SetBool("hasRole", false);

            if (isLocalPlayer)
            {
                HideSpyIndicators();
            }
        }
        else if (alignment == AgentAlignment.Agent)
        {
            playerUI.alignmentBackground.color = alignmentColorAgent;
            playerUI.roleAnimator.SetBool("hasRole", true);

            if (isLocalPlayer)
            {
                HideSpyIndicators();
            }
        }
        else if (alignment == AgentAlignment.Spy)
        {
            playerUI.alignmentBackground.color = alignmentColorSpy;
            playerUI.roleAnimator.SetBool("hasRole", true);
            spyIndicator.SetActive(true);

            if (isLocalPlayer)
            {
                ShowSpyIndicators();
            }
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
        coatRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        coatTrimRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
    }

    public void ShowLocalPlayer()
    {
        playerRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        coatRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        coatTrimRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }

    public void ShowSpyIndicators()
    {
        Camera.main.cullingMask = spyCullingMask;
    }

    public void HideSpyIndicators()
    {
        Camera.main.cullingMask = baseCullingMask;
    }
}

public enum AgentAlignment
{
    Unassigned,
    Agent,
    Spy
}


