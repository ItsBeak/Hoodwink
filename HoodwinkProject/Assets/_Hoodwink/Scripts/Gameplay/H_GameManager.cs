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
    public AgentData[] agentData;
    public Color[] pantsColours, shoesColours;
    List<AgentData> availableAgents;

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
        availableAgents = new List<AgentData>();

        foreach (AgentData agent in agentData)
        {
            availableAgents.Add(agent);
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

        int randomAgent = Random.Range(0, availableAgents.Count);

        player.playerName = availableAgents[randomAgent].agentName;
        player.shirtColour = availableAgents[randomAgent].agentColour;

        player.pantsColour = pantsColours[Random.Range(0, pantsColours.Length)];
        player.shoesColour = shoesColours[Random.Range(0, shoesColours.Length)];

        availableAgents.RemoveAt(randomAgent);

    }

    [Command(requiresAuthority = false)]
    public void CmdUnregisterPlayer(H_PlayerBrain player)
    {
        AgentData newAgentData = new AgentData();

        newAgentData.agentName = player.playerName;
        newAgentData.agentColour = player.shirtColour;

        availableAgents.Add(newAgentData);

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

[System.Serializable]
public struct AgentData
{
    public string agentName;
    public Color agentColour;
}