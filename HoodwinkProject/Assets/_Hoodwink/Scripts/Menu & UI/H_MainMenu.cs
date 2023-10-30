using UnityEngine;
using UnityEngine.UI;

using Mirror;

using TMPro;

using UnityEngine.Timeline;
using UnityEngine.Playables;

public class H_MainMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject loginButton = null;
    public GameObject hostButton = null;
    public GameObject joinButton = null;
    public TMP_InputField codeInputField;

    [Header("Directors")]
    public PlayableDirector preLoginDirector, loggedInDirector, customizationDirector, customizationEndDirector, optionsDirector, optionsEndDirector, startGameDirector;

    public float startGameDelay;

    bool playedLogIn = false;

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
        if (FindObjectOfType<H_Bootstrap>().hasPressedPlay)
        {
            loggedInDirector.gameObject.SetActive(true);
            GetComponent<AudioSource>().Play();
        }
        else
        {
            preLoginDirector.gameObject.SetActive(true);
        }
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space) && preLoginDirector.time < 14f)
        {
            preLoginDirector.time = 14f;
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
        FindObjectOfType<H_Bootstrap>().hasPressedPlay = true;
        loggedInDirector.gameObject.SetActive(true);
    }

    public void HostButton()
    {
        startGameDirector.gameObject.SetActive(true);
        Invoke("StartHost", startGameDelay);
    }

    public void StartHost()
    {
        H_TransitionManager.instance.SetBlack();
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
