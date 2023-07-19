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
    public PlayableDirector preLoginDirector, loggingInDirector, mainMenuDirector;
    public float returnToMenuTime;

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

    private void Update()
    {
        loginButton.SetActive(!NetManager.isLoggedIn);
        hostButton.SetActive(NetManager.isLoggedIn);
        joinButton.SetActive(NetManager.isLoggedIn);
        codeInputField.gameObject.SetActive(NetManager.isLoggedIn);

        loginButton.GetComponent<Button>().interactable = !NetManager.isLoggingIn;

        if (NetManager.isLoggedIn)
        {

            if (!playedLogIn)
            {
                Debug.Log("First time logging in");

                playedLogIn = true;
                loggingInDirector.gameObject.SetActive(true);
            }
            else if (false) // make this check if the player has logged in already, and is returning to the menu
            {
                //playedLogIn = true;
                //
                //Debug.Log("Player already logged in, adjusting intro cinematic");
                //
                //preLoginDirector.initialTime = 10.5f;
                //preLoginDirector.time = 10.5f;
                //
                //loggingInDirector.gameObject.SetActive(true);
                //loggingInDirector.initialTime = 1.65f;
                //loggingInDirector.time = 1.65f;
            }


        }



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
