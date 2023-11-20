using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using Mono.Cecil.Cil;

public class H_PlayerUI : MonoBehaviour
{
    [Header("Canvas Groups")]
    public CanvasGroup gameUI;
    public CanvasGroup playerUI;
    public CanvasGroup spectatorUI;
    public CanvasGroup pauseUI;
    public CanvasGroup hotbar;

    [Header("Alignment Data")]
    public Color alignmentColorUnassigned;
    public Color alignmentColorAgent;
    public Color alignmentColorSpy; 
    public TextMeshProUGUI alignmentText, alignmentTextShadow;
    public Image alignmentBackground;

    [Header("Player GUI")]
    public Animator folderAnimator;
    public Animator gadgetCardsAnimator;
    public Animator slotPrimaryAnimator, slotSidearmAnimator, slotFirstGadgetAnimator, slotSecondGadgetAnimator;

    public GameObject firstGadget, secondGadget;

    public Image staminaBarImage;

    [Header("Components")]
    H_PlayerBrain brain;
    public TextMeshProUGUI spectatedAgentText;

    [HideInInspector] public bool isOpen;

    private H_NetworkManager nm;

    private H_NetworkManager NetManager
    {
        get
        {
            if (nm != null) { return nm; }
            return nm = NetworkManager.singleton as H_NetworkManager;
        }
    }
    private void Start()
    {
        brain = GetComponentInParent<H_PlayerBrain>();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isOpen)
            {
                OpenPauseMenu();
            }
            else
            {
                ClosePauseMenu();
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            folderAnimator.SetBool("FolderOpen", !folderAnimator.GetBool("FolderOpen"));
            gadgetCardsAnimator.SetBool("isOpen", folderAnimator.GetBool("FolderOpen"));
        }

    }

    public void OpenPauseMenu()
    {
        isOpen = true;

        brain.isPaused = true;

        gameUI.alpha = 0;
        gameUI.interactable = false;
        gameUI.blocksRaycasts = false;

        pauseUI.alpha = 1;
        pauseUI.interactable = true;
        pauseUI.blocksRaycasts = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ClosePauseMenu()
    {
        isOpen = false;

        brain.isPaused = false;

        pauseUI.alpha = 0;
        pauseUI.interactable = false;
        pauseUI.blocksRaycasts = false;

        gameUI.alpha = 1;
        gameUI.interactable = true;
        gameUI.blocksRaycasts = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Disconnect()
    {
        if (GetComponentInParent<NetworkIdentity>().isServer)
        {
            NetManager.StopHost();
        }
        else
        {
            NetManager.StopClient();
        }
    }

    public void ShowSpectatorUI()
    {
        playerUI.alpha = 0;
        playerUI.interactable = false;
        playerUI.blocksRaycasts = false;

        spectatorUI.alpha = 1;
        spectatorUI.interactable = true;
        spectatorUI.blocksRaycasts = true;
    }

    public void ShowGameUI()
    {
        spectatorUI.alpha = 0;
        spectatorUI.interactable = false;
        spectatorUI.blocksRaycasts = false;

        playerUI.alpha = 1;
        playerUI.interactable = true;
        playerUI.blocksRaycasts = true;
    }

    public void ChangeSpectator(string spectatorName)
    {
        spectatedAgentText.text = spectatorName;
    }

    public void ShowHotbar(bool state)
    {
        hotbar.alpha = state ? 1 : 0;
    }

    public void ShowGadgets(bool state)
    {
        firstGadget.SetActive(state);
        secondGadget.SetActive(state);
    }

}
