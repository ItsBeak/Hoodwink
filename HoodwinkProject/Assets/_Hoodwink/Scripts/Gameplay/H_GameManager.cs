using UnityEngine;
using Mirror;
using TMPro;
using System.Collections.Generic;

public class H_GameManager : NetworkBehaviour
{
    public static H_GameManager instance { get; private set; }

    [Header("Game Data")]
    [HideInInspector] public string relayCode;
    [HideInInspector] public List<H_PlayerBrain> serverPlayers;

    [Header("Components")]
    public TextMeshProUGUI relayCodeUI;
    public TextMeshProUGUI pingDisplay;

    [Header("Player Colours")]
    public Color[] shirtColours, pantsColours, bootsColours;
    List<Color> availableColours;

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

    public override void OnStartServer()
    {
        availableColours = new List<Color>();

        foreach (Color c in shirtColours)
        {
            availableColours.Add(c);
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void Update()
    {
        DisplayPing();
    }

    [Command(requiresAuthority = false)]
    public void CmdRegisterPlayer(H_PlayerBrain player)
    {
        serverPlayers.Add(player);

        int randomColour = Random.Range(0, availableColours.Count);

        player.shirtColour = availableColours[randomColour];

        availableColours.RemoveAt(randomColour);

    }

    [Command(requiresAuthority = false)]
    public void CmdUnregisterPlayer(H_PlayerBrain player)
    {
        availableColours.Add(player.shirtColour);

        serverPlayers.Remove(player);
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