using UnityEngine;
using UnityEngine.UI;

using Mirror;

using TMPro;

public class H_MainMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject loginButton = null;
    public GameObject hostButton = null;
    public GameObject joinButton = null;
    public TMP_InputField codeInputField;

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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            JoinButton();
        }
    }

    public void LoginButton()
    {
        NetManager.UnityLogin();
    }

    public void HostButton()
    {
        NetManager.StartRelayHost(NetManager.maxConnections, "australia-southeast1");
    }

    public void JoinButton()
    {
        NetManager.relayJoinCode = codeInputField.text;

        NetManager.JoinRelayServer();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
