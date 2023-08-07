using UnityEngine;
using UnityEngine.UI;

using Mirror;

using TMPro;

using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.LowLevel;

public class H_MainMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject loginButton = null;
    public GameObject hostButton = null;
    public GameObject joinButton = null;
    public TMP_InputField codeInputField;

    [Header("Directors")]
    public PlayableDirector preLoginDirector, loggedInDirector, customizationDirector, customizationEndDirector, optionsDirector, optionsEndDirector;

    bool playedLogIn = false;
    bool startup = false;

    private H_NetworkManager nm;

    private H_NetworkManager NetManager
    {
        get
        {
            if (nm != null) { return nm; }
            return nm = NetworkManager.singleton as H_NetworkManager;
        }
    }

    private void Update()
    {
        loginButton.SetActive(!NetManager.isLoggedIn);
        hostButton.SetActive(NetManager.isLoggedIn);
        joinButton.SetActive(NetManager.isLoggedIn);
        codeInputField.gameObject.SetActive(NetManager.isLoggedIn);

        loginButton.GetComponent<Button>().interactable = !NetManager.isLoggingIn;

        if (Input.GetKeyDown(KeyCode.Space) && preLoginDirector.time < 9f)
        {
            preLoginDirector.time = 9f;
        }

        if (!startup)
        {
            if (NetManager.isLoggedIn)
            {
                loggedInDirector.gameObject.SetActive(true);
                GetComponent<AudioSource>().Play();
            }
            else
            {
                preLoginDirector.gameObject.SetActive(true);
            }

            startup = true;

        }

        if (NetManager.isLoggedIn)
        {
            
            if (!playedLogIn)
            {
                playedLogIn = true;
                loggedInDirector.gameObject.SetActive(true);
            }
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            JoinButton();
        }
    }

    public void PlayButton()
    {
        NetManager.isLoggedIn = true;
    }

    public void HostButton()
    {
        NetManager.StartHost();
    }

    public void JoinButton()
    {
        NetManager.networkAddress = codeInputField.text;
        NetManager.StartClient();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenCustomizationMenu()
    {
        customizationDirector.gameObject.SetActive(true);
        customizationEndDirector.gameObject.SetActive(false);
    }

    public void CloseCustomizationMenu()
    {
        customizationDirector.gameObject.SetActive(false);
        customizationEndDirector.gameObject.SetActive(true);

    }

    public void OpenOptionsMenu()
    {
        optionsDirector.gameObject.SetActive(true);
        optionsEndDirector.gameObject.SetActive(false);
    }

    public void CloseOptionsMenu()
    {
        optionsDirector.gameObject.SetActive(false);
        optionsEndDirector.gameObject.SetActive(true);

    }
}
