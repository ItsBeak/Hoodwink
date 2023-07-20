using UnityEngine;
using Mirror;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class H_GameManager : NetworkBehaviour
{
    public static H_GameManager instance { get; private set; }

    [Header("Game Data")]
    [HideInInspector] public string relayCode;

    [HideInInspector] public List<H_PlayerBrain> serverPlayers;
    [HideInInspector] public List<H_PlayerBrain> roundPlayers;
    [HideInInspector] public List<H_PlayerBrain> roundDeadPlayers;
    [HideInInspector] public List<H_PlayerBrain> roundAgents;
    [HideInInspector] public List<H_PlayerBrain> roundSpies;

    [HideInInspector] public string chosenScene;
    bool winConditionMet = false;
    H_LevelData currentLevel;

    [HideInInspector, SyncVar] public RoundStage currentRoundStage = RoundStage.Lobby;
    [HideInInspector, SyncVar] public WinConditions winCondition;

    [Header("Lobby Settings")]
    public Transform[] lobbySpawns;

    [Header("Player Settings")]
    [Range(3, 8)]public int minPlayersToStart = 3;
    public PlayerSettings[] playerSettings;

    int roundSpiesRemaining = 0;

    [Header("Player Colours")]
    public AgentData[] agentData;
    public Color[] shirtColours, pantsColours, shoesColours;
    List<AgentData> availableAgents;

    [Header("Map Pool")]
    [Scene] public string[] maps;

    [Header("Round Timer Settings")]
    public float warmupLength;
    [SyncVar] float warmupTimer;
    public float roundLength;
    [SyncVar] float roundTimer;
    public float postGameLength;
    [SyncVar] float postGameTimer;

    [Header("Components")]
    public TextMeshProUGUI relayCodeUI;
    public TextMeshProUGUI pingDisplay;
    public TextMeshProUGUI timerDisplay;

    [Header("Debugging")]
    public bool enableDebugLogs;
    public bool overrideMinimumPlayerCount;
    public PlayerSettings overrideSettings;

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

        switch (currentRoundStage)
        {
            #region Lobby Stage Update
            case RoundStage.Lobby:

                timerDisplay.text = "Waiting for game to start...";

                if (!isServer)
                    return;

                CheckReadyPlayers();

                break;
            #endregion

            #region Warmup Stage Update
            case RoundStage.Warmup:

                timerDisplay.text = "Warmup ends in " + Mathf.RoundToInt(warmupTimer + 0.5f).ToString() + " seconds";

                if (!isServer)
                    return;

                warmupTimer -= 1 * Time.deltaTime;

                if (warmupTimer <= 0)
                {
                    currentRoundStage = RoundStage.Game;
                    AssignRoles();
                }

                break;
            #endregion

            #region Game Stage Update
            case RoundStage.Game:

                timerDisplay.text = "In game, time left: " + Mathf.RoundToInt(roundTimer + 0.5f).ToString();

                if (!isServer)
                    return;

                roundTimer -= 1 * Time.deltaTime;

                if (winConditionMet)
                {
                    currentRoundStage = RoundStage.PostGame;
                    //RpcShowRoundResults();
                }

                if (roundTimer <= 0)
                {
                    CheckWinConditions();
                    currentRoundStage = RoundStage.PostGame;
                    //RpcShowRoundResults();
                }

                break;
            #endregion

            #region Post Game Stage Update
            case RoundStage.PostGame:

                timerDisplay.text = "Game over, returning to lobby in : " + Mathf.RoundToInt(postGameTimer + 0.5f).ToString() + " seconds";

                if (!isServer)
                    return;

                postGameTimer -= 1 * Time.deltaTime;

                if (postGameTimer <= 0)
                {
                    ResetRoles();
                    ResetPlayerStates();
                    EndRound();
                }

                break;
            #endregion

            default: break;
        }

    }

    [Command(requiresAuthority = false)]
    public void CmdRegisterPlayer(H_PlayerBrain player)
    {
        serverPlayers.Add(player);

        player.playerName = "anon";

        player.shirtColour = shirtColours[Random.Range(0, shirtColours.Length)];
        player.pantsColour = pantsColours[Random.Range(0, pantsColours.Length)];
        player.shoesColour = shoesColours[Random.Range(0, shoesColours.Length)];
    }

    [Command(requiresAuthority = false)]
    public void CmdUnregisterPlayer(H_PlayerBrain player)
    {
        AgentData newAgentData = new AgentData();

        if (player.hasAgentData)
        {
            newAgentData.agentName = player.playerName;
            newAgentData.agentColour = player.shirtColour;

            availableAgents.Add(newAgentData);
        }

        serverPlayers.Remove(player);
        roundPlayers.Remove(player);
        roundAgents.Remove(player);
        roundSpies.Remove(player);
        roundDeadPlayers.Remove(player);

        CheckWinConditions();

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

    [Server]
    void CheckReadyPlayers()
    {
        bool allPlayersReady = true;

        foreach (var player in serverPlayers)
        {
            if (!player.isReady)
            {
                allPlayersReady = false;
                break;
            }
        }

        if (allPlayersReady)
        {
            if (minPlayersToStart > serverPlayers.Count)
            {
                if (!overrideMinimumPlayerCount)
                {
                    timerDisplay.text = "Not enough players to start: " + serverPlayers.Count + "/" + minPlayersToStart;
                    return;
                }
            }

            StartRound();
        }

    }

    [Server]
    public void StartRound()
    {
        StartCoroutine(InitializeLevel());

        warmupTimer = warmupLength;
        roundTimer = roundLength;
        postGameTimer = postGameLength;

        foreach (var player in serverPlayers)
        {
            roundPlayers.Add(player);
            player.isReady = false;
        }

        ResetPlayerStates();
        ResetRoles();

        currentRoundStage = RoundStage.Warmup;

    }

    [Server]
    public void EndRound()
    {
        foreach (var player in serverPlayers)
        {
            player.isReady = false;
            //player.GetComponent<H_PlayerEquipment>().CmdDropItem();
        }

        foreach (var player in roundPlayers)
        {
            int randomSpawn = Random.Range(0, lobbySpawns.Length);

            player.TeleportPlayer(lobbySpawns[randomSpawn].position, lobbySpawns[randomSpawn].rotation);

            AgentData newAgentData = new AgentData();

            newAgentData.agentName = player.playerName;
            newAgentData.agentColour = player.shirtColour;

            availableAgents.Add(newAgentData);

            player.hasAgentData = false;

            player.playerName = "anon";
            player.shirtColour = shirtColours[Random.Range(0, shirtColours.Length)];

        }

        roundPlayers.Clear();
        roundAgents.Clear();
        roundSpies.Clear();

        winConditionMet = false;

        Invoke("CleanupObjects", 1f);

        RpcUnloadMap(chosenScene);
        currentRoundStage = RoundStage.Lobby;

    }

    IEnumerator InitializeLevel()
    {
        chosenScene = maps[Random.Range(0, maps.Length)];

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(chosenScene, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            if (enableDebugLogs)
                Debug.LogWarning("Loading Level: " + chosenScene);
            yield return null;
        }

        if (enableDebugLogs)
            Debug.LogWarning("Level Loaded: " + chosenScene);

        RpcLoadMap(chosenScene);

        yield return new WaitForSeconds(2);

        currentLevel = FindObjectOfType<H_LevelData>();

        Transform[] spawns = currentLevel.playerSpawnPoints;

        foreach (var player in roundPlayers)
        {
            int randomSpawn = Random.Range(0, spawns.Length);

            player.TeleportPlayer(spawns[randomSpawn].position, spawns[randomSpawn].rotation);

            int randomAgent = Random.Range(0, availableAgents.Count);

            player.playerName = availableAgents[randomAgent].agentName;
            player.shirtColour = availableAgents[randomAgent].agentColour;

            player.pantsColour = pantsColours[Random.Range(0, pantsColours.Length)];
            player.shoesColour = shoesColours[Random.Range(0, shoesColours.Length)];

            availableAgents.RemoveAt(randomAgent);

            player.hasAgentData = true;
        }


    }

    [Server]
    private void AssignRoles()
    {
        int totalPlayers = roundPlayers.Count;

        PlayerSettings newRoundSettings = new PlayerSettings();

        if (overrideMinimumPlayerCount)
        {
            newRoundSettings = overrideSettings;
        }
        else
        {
            foreach (PlayerSettings settings in playerSettings)
            {
                if (settings.playerCount == totalPlayers)
                {
                    newRoundSettings = settings;
                    break;
                }
            }
        }

        roundSpiesRemaining = newRoundSettings.spyCount;

        while (roundSpiesRemaining > 0)
        {
            int randomPlayerIndex = Random.Range(0, totalPlayers);

            if (roundPlayers[randomPlayerIndex].currentAlignment != AgentAlignment.Spy)
            {
                serverPlayers[randomPlayerIndex].currentAlignment = AgentAlignment.Spy;
                roundSpies.Add(serverPlayers[randomPlayerIndex]);
                roundSpiesRemaining--;
            }
        }

        for (int i = 0; i < totalPlayers; i++)
        {
            if (serverPlayers[i].currentAlignment != AgentAlignment.Spy)
            {
                serverPlayers[i].currentAlignment = AgentAlignment.Agent;
                roundAgents.Add(serverPlayers[i]);
            }
        }

    }

    [ClientRpc]
    void RpcLoadMap(string scene)
    {
        if (!isServer)
            SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
    }

    [ClientRpc]
    void RpcUnloadMap(string scene)
    {
        SceneManager.UnloadSceneAsync(scene);
    }

    [Server]
    private void ResetPlayerStates()
    {
        foreach (var player in serverPlayers)
        {
            //player.GetComponent<H_PlayerHealth>().isDead = false;
            //player.GetComponent<H_PlayerHealth>().FullHeal();
        }
    }

    [Server]
    private void ResetRoles()
    {
        foreach (var player in serverPlayers)
        {
            player.currentAlignment = AgentAlignment.Unassigned;
        }
    }

    [Server]
    void CheckWinConditions()
    {
        if (winConditionMet)
            return;

        if (roundAgents.Count == 0 && roundSpies.Count == 0)
        {
            winCondition = WinConditions.Draw;
            winConditionMet = true;
        }
        else if (roundSpies.Count == 0)
        {
            winCondition = WinConditions.GoodWin;
            winConditionMet = true;
        }
        else if (roundAgents.Count == 0)
        {
            winCondition = WinConditions.EvilWin;
            winConditionMet = true;
        }
        else if (roundTimer <= 0)
        {
            winCondition = WinConditions.TimeOut;
            winConditionMet = true;
        }

    }
}

[System.Serializable]
public struct AgentData
{
    public string agentName;
    public Color agentColour;
}

[System.Serializable]
public class PlayerSettings
{
    public int playerCount;
    public int spyCount;
}

public enum RoundStage
{
    Lobby,
    Warmup,
    Game,
    PostGame,
}

public enum WinConditions
{
    GoodWin,
    EvilWin,
    TimeOut,
    Draw
}