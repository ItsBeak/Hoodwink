using UnityEngine;
using Mirror;
using TMPro;

public class H_GameManager : MonoBehaviour
{
    [Header("Game Data")]
    [HideInInspector] public string relayCode;

    [Header("Components")]
    public TextMeshProUGUI relayCodeUI;
    public TextMeshProUGUI pingDisplay;

    [Header("Debugging")]
    public bool enableDebugLogs;

    private H_NetworkManager netManager;

    private H_NetworkManager NetManager
    {
        get
        {
            if (netManager != null) { return netManager; }
            return netManager = NetworkManager.singleton as H_NetworkManager;
        }
    }

    void Start()
    {
        relayCodeUI.text = "Join Code: " + NetManager.relayJoinCode.ToUpper();
    }

    private void Update()
    {

        DisplayPing();

    }

    void DisplayPing()
    {
        if (NetworkClient.active)
        {
            float ping = Mathf.Round((float)(NetworkTime.rtt * 1000));
            pingDisplay.text = "Ping: " + ping + "ms";

            if (ping > 200)
            {
                pingDisplay.color = Color.red;
            }
            else if (ping > 100)
            {
                pingDisplay.color = Color.yellow;
            }
            else if (ping > 1)
            {
                pingDisplay.color = Color.green;
            }
            else
            {
                pingDisplay.color = Color.white;
                pingDisplay.text = "Ping: Host";
            }

        }
    }

}