using UnityEngine;
using Mirror;
using Cinemachine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class H_PlayerBrain : NetworkBehaviour
{
    [Header("Player States & Settings")]
    public bool canMove = true;
    public bool canSprint;
    public bool canJump;
    [HideInInspector] public bool isPaused;
    public float speedMultiplier = 1;

    [Header("Player Cosmetic Data")]
    [SyncVar(hook = nameof(OnPlayerNameChanged))] public string playerName;
    [SyncVar(hook = nameof(OnAgentDataChanged))] public AgentData agentData;
    [SyncVar(hook = nameof(OnReadyChanged))] public bool isReady = false;
    [SyncVar(hook = nameof(OnHudVisibilityChanged))] public bool isHudHidden = false;
    //[SyncVar] public IntroCosmeticData cosmeticData;

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
    public H_PlayerCosmetics cosmetics;

    [Header("Rendering")]
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

    public override void OnStartLocalPlayer()
    {
        int hatIndex, suitIndex, vestIndex;
        hatIndex = H_CosmeticManager.instance.currentHat.ID;
        suitIndex = H_CosmeticManager.instance.currentSuitCut;
        vestIndex = H_CosmeticManager.instance.currentVestCut;

        CmdSetPlayerCosmetics(hatIndex, suitIndex, vestIndex);
        CmdSetPlayerName(PlayerPrefs.GetString("C_SELECTED_NAME", "Hoodwinker"));
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isLocalPlayer)
        {
            H_TransitionManager.instance.FadeOut(0.25f);

            cam.enabled = true;
            playerUI.gameObject.SetActive(true);

            cosmetics.HidePlayer();

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

    public void OnAgentDataChanged(AgentData oldData, AgentData newData)
    {
        agentNameText.text = newData.agentName;

        agentColourImage.color = newData.primaryColour;

        cosmetics.SetHat(newData.hatIndex);
        cosmetics.ToggleSuit(newData.suitIndex);
        cosmetics.ToggleVest(newData.vestIndex);

        cosmetics.SetJacketColour(newData.primaryColour);
        cosmetics.SetPantsColour(newData.pantsColour);
        cosmetics.SetVestColour(newData.vestColour);
        cosmetics.SetTieColour(newData.primaryColour);
        cosmetics.SetCollarColour(newData.secondaryColour);
        cosmetics.SetPocketColour(newData.secondaryColour);
    }

    public void OnPlayerNameChanged(string oldName, string newName)
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
    }


    void OnAlignmentChanged(AgentAlignment oldRole, AgentAlignment newRole)
    {
        UpdateAlignmentUI(newRole);
    }

    void UpdateAlignmentUI(AgentAlignment alignment)
    {
        playerUI.alignmentText.text = currentAlignment.ToString();
        spyIndicator.SetActive(false);

        if (alignment == AgentAlignment.Unassigned)
        {
            playerUI.alignmentBackground.color = alignmentColorUnassigned;
            playerUI.roleAnimator.SetTrigger("Reset");

            if (isLocalPlayer)
            {
                HideSpyIndicators();
            }
        }
        else if (alignment == AgentAlignment.Agent)
        {
            playerUI.alignmentBackground.color = alignmentColorAgent;
            playerUI.roleAnimator.SetTrigger("ID Card");

            if (isLocalPlayer)
            {
                HideSpyIndicators();
            }
        }
        else if (alignment == AgentAlignment.Spy)
        {
            playerUI.alignmentBackground.color = alignmentColorSpy;
            playerUI.roleAnimator.SetTrigger("ID Card");
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

    [Command]
    void CmdSetPlayerCosmetics(int hatID, int suitID, int vestID)
    {
        agentData.hatIndex = hatID;
        agentData.suitIndex = suitID;
        agentData.vestIndex = vestID;
        RpcUpdateCosmetics();
    }

    [ClientRpc]
    void RpcUpdateCosmetics()
    {
        cosmetics.SetHat(agentData.hatIndex);
        cosmetics.ToggleSuit(agentData.suitIndex);
        cosmetics.ToggleVest(agentData.vestIndex);
    }

    [Command]
    void CmdSetPlayerName(string newName)
    {
        playerName = newName;
    }

    public void ShowSpyIndicators()
    {
        Camera.main.cullingMask = spyCullingMask;
    }

    public void HideSpyIndicators()
    {
        Camera.main.cullingMask = baseCullingMask;
    }

    void OnHudVisibilityChanged(bool oldValue, bool newValue)
    {
        playerUI.playerUI.alpha = isHudHidden ? 0 : 1;
    }

    [ClientRpc]
    public void RpcPlayAgentIntro(IntroCosmeticData player)
    {
        if (isLocalPlayer)
        {
            H_CinematicManager.instance.PlayAgentIntro(player);
        }
    }

    [ClientRpc]
    public void RpcPlaySpyIntro(List<IntroCosmeticData> players)
    {
        if (isLocalPlayer)
        {
            H_CinematicManager.instance.PlaySpyIntro(players);
        }
    }
}

public enum AgentAlignment
{
    Unassigned,
    Agent,
    Spy
}


