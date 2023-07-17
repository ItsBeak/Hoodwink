using UnityEngine;
using Mirror;

public class H_PlayerMenu : MonoBehaviour
{
    [Header("Canvas Groups")]
    public CanvasGroup gameUI;
    public CanvasGroup pauseUI;

    [Header("Components")]
    H_PlayerBrain brain;

    bool isOpen;

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
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }
}
