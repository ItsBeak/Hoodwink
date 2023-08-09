using UnityEngine;
using Mirror;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class H_GameManager : NetworkBehaviour
{
    public static H_GameManager instance { get; private set; }

    [Header("Evidence Data")]
    [SyncVar(hook = nameof(OnEvidenceChanged))] int evidencePercent = 0;
    [SyncVar(hook = nameof(OnSpyInformationChanged))] string spyInformation = "";
    public TextMeshProUGUI evidenceText;
    public TextMeshProUGUI revealedSpiesText;
    public Image evidenceImage;
    [SyncVar] bool spiesRevealed = false;

    public List<H_PlayerBrain> serverPlayers;
    public List<H_PlayerBrain> roundPlayers;
    public List<H_PlayerBrain> roundDeadPlayers;
    public List<H_PlayerBrain> roundAgents;
    public List<H_PlayerBrain> roundSpies;

    [HideInInspector] public string chosenScene;
    bool winConditionMet = false;
    [HideInInspector] public H_LevelData currentLevel;

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
    public Color[] coatColours, trimColours, pantsColours, shoesColours;
    List<AgentData> allAgents;
    List<AgentData> availableAgents;

    [Header("Default Sidearm Settings")]
    public GameObject defaultClientSidearm;
    public GameObject defaultObserverSidearm;
    public GameObject boratGunClient; // remove these dear god
    public GameObject boratGunObserver; // dont you forget to remove these

    [Header("Default Sidearm Settings")]
    public GameObject defaultClientHolstered;
    public GameObject defaultObserverHolstered;

    [Header("Gadgets")]
    public GameObject[] spyGadgets;
    public GameObject[] agentGadgets;

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
    public TextMeshProUGUI pingDisplay;
    public TextMeshProUGUI timerDisplay;
    H_ObjectManager objectManager;

    [Header("Debugging")]
    public bool enableDebugLogs;
    public bool overrideMinimumPlayerCount;
    public bool allowServerToSkipGame;
    public PlayerSettings overrideSettings;
    PlayerSettings currentSettings;

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
        objectManager = GetComponent<H_ObjectManager>();
        revealedSpiesText.text = "";
    }

    public override void OnStartServer()
    {
        allAgents = new List<AgentData>();
        availableAgents = new List<AgentData>();

        foreach (AgentData agent in agentData)
        {
            allAgents.Add(agent);
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
                    winConditionMet = false;
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

                if (Input.GetKeyDown(KeyCode.Backspace) && allowServerToSkipGame)
                {
                    roundTimer -= 999;
                }

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

        if (Input.GetKeyDown(KeyCode.L))
        {
            CmdUpdateEvidence(10);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            CmdUpdateEvidence(-10);
        }

    }

    [Command(requiresAuthority = false)]
    public void CmdRegisterPlayer(H_PlayerBrain player)
    {
        serverPlayers.Add(player);

        player.playerName = "Anonymous";

        player.coatColour = coatColours[Random.Range(0, coatColours.Length)];
        player.coatTrimColour = trimColours[Random.Range(0, trimColours.Length)];
        player.pantsColour = pantsColours[Random.Range(0, pantsColours.Length)];
        player.shoesColour = shoesColours[Random.Range(0, shoesColours.Length)];
    }

    [Command(requiresAuthority = false)]
    public void CmdUnregisterPlayer(H_PlayerBrain player)
    {
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

        if (overrideMinimumPlayerCount)
        {
            currentSettings = overrideSettings;
        }
        else
        {
            foreach (PlayerSettings settings in playerSettings)
            {
                if (settings.playerCount == roundPlayers.Count)
                {
                    currentSettings = settings;
                    break;
                }
            }
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
            player.equipment.RpcTryDropItem();
        }

        foreach (var player in roundPlayers)
        {

            int randomSpawn = Random.Range(0, lobbySpawns.Length);

            player.TeleportPlayer(lobbySpawns[randomSpawn].position, lobbySpawns[randomSpawn].rotation);

            player.playerName = "Anonymous";
            player.coatColour = coatColours[Random.Range(0, coatColours.Length)];
            player.coatTrimColour = trimColours[Random.Range(0, trimColours.Length)];

            NetworkServer.Destroy(player.equipment.currentGadget.gameObject);

            NetworkServer.Destroy(player.equipment.sidearmClientObject.gameObject);
            NetworkServer.Destroy(player.equipment.sidearmObserverObject.gameObject);

            NetworkServer.Destroy(player.equipment.holsteredClientObject.gameObject);
            NetworkServer.Destroy(player.equipment.holsteredObserverObject.gameObject);

            player.equipment.RpcClearSidearmSlot();
            player.equipment.RpcClearHolsteredSlot();
        }

        roundPlayers.Clear();
        roundAgents.Clear();
        roundSpies.Clear();
        roundDeadPlayers.Clear();

        winConditionMet = false;

        availableAgents.Clear();

        evidencePercent = 0;
        spiesRevealed = false;

        spyInformation = "";

        Invoke("CleanupObjects", 0.3f);

        RpcUnloadMap(chosenScene);
        currentRoundStage = RoundStage.Lobby;

    }

    void CleanupObjects()
    {
        objectManager.CleanupObjects();
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

        foreach (AgentData agent in allAgents)
        {
            availableAgents.Add(agent);
        }

        foreach (var player in roundPlayers)
        {
            player.equipment.RpcTryDropItem();

            int randomSpawn = Random.Range(0, spawns.Length);

            player.TeleportPlayer(spawns[randomSpawn].position, spawns[randomSpawn].rotation);

            int randomAgent = Random.Range(0, availableAgents.Count);

            player.playerName = availableAgents[randomAgent].agentName;
            player.coatColour = availableAgents[randomAgent].agentSecondaryColour;
            player.coatTrimColour = availableAgents[randomAgent].agentColour;

            player.pantsColour = pantsColours[Random.Range(0, pantsColours.Length)];
            player.shoesColour = shoesColours[Random.Range(0, shoesColours.Length)];

            availableAgents.RemoveAt(randomAgent);

        }

        Debug.Log("Spawning objects");

        objectManager.SpawnObjects(currentSettings);

    }

    [Server]
    private void AssignRoles()
    {
        roundSpiesRemaining = currentSettings.spyCount;

        while (roundSpiesRemaining > 0)
        {
            int randomPlayerIndex = Random.Range(0, roundPlayers.Count);

            if (roundPlayers[randomPlayerIndex].currentAlignment != AgentAlignment.Spy)
            {
                roundPlayers[randomPlayerIndex].currentAlignment = AgentAlignment.Spy;
                roundSpies.Add(roundPlayers[randomPlayerIndex]);
                roundSpiesRemaining--;

                GameObject newGadget = Instantiate(spyGadgets[Random.Range(0, spyGadgets.Length)]);
                NetworkServer.Spawn(newGadget, roundPlayers[randomPlayerIndex].connectionToClient);
                roundPlayers[randomPlayerIndex].equipment.RpcEquipGadget(newGadget);
            }
        }

        for (int i = 0; i < roundPlayers.Count; i++)
        {
            if (roundPlayers[i].currentAlignment != AgentAlignment.Spy)
            {
                roundPlayers[i].currentAlignment = AgentAlignment.Agent;
                roundAgents.Add(roundPlayers[i]);

                GameObject newGadget = Instantiate(agentGadgets[Random.Range(0, agentGadgets.Length)]);
                NetworkServer.Spawn(newGadget, roundPlayers[i].connectionToClient);
                roundPlayers[i].equipment.RpcEquipGadget(newGadget);
            }

            int boratChance = Random.Range(0, 100); // dear god remove this

            if (boratChance == 50)
            {
                GameObject newClientSidearm = Instantiate(boratGunClient);
                GameObject newObserverSidearm = Instantiate(boratGunObserver);

                NetworkServer.Spawn(newClientSidearm, roundPlayers[i].connectionToClient);
                NetworkServer.Spawn(newObserverSidearm, roundPlayers[i].connectionToClient);

                roundPlayers[i].equipment.RpcEquipSidearm(newClientSidearm, newObserverSidearm);
            }
            else
            {
                GameObject newClientSidearm = Instantiate(defaultClientSidearm);
                GameObject newObserverSidearm = Instantiate(defaultObserverSidearm);

                NetworkServer.Spawn(newClientSidearm, roundPlayers[i].connectionToClient);
                NetworkServer.Spawn(newObserverSidearm, roundPlayers[i].connectionToClient);

                roundPlayers[i].equipment.RpcEquipSidearm(newClientSidearm, newObserverSidearm);
            }

            GameObject newClientHolstered = Instantiate(defaultClientHolstered);
            GameObject newObserverHolstered = Instantiate(defaultObserverHolstered);

            NetworkServer.Spawn(newClientHolstered, roundPlayers[i].connectionToClient);
            NetworkServer.Spawn(newObserverHolstered, roundPlayers[i].connectionToClient);

            roundPlayers[i].equipment.RpcEquipHolstered(newClientHolstered, newObserverHolstered);

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
            player.GetComponent<H_PlayerHealth>().isDead = false;
            player.GetComponent<H_PlayerHealth>().FullHeal();
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

    [Command(requiresAuthority = false)]
    public void CmdPlayerKilled(H_PlayerBrain player)
    {
        if (currentRoundStage == RoundStage.Game)
        {
            if (player.currentAlignment == AgentAlignment.Agent)
            {
                roundAgents.Remove(player);
                roundDeadPlayers.Add(player);
            }
            else if (player.currentAlignment == AgentAlignment.Spy)
            {
                roundSpies.Remove(player);
                roundDeadPlayers.Add(player);
            }

            CheckWinConditions();

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
            Debug.Log("Win Condition Met: Draw");
        }
        else if (roundSpies.Count == 0)
        {
            winCondition = WinConditions.GoodWin;
            winConditionMet = true;
            Debug.Log("Win Condition Met: Agents Win");
        }
        else if (roundAgents.Count == 0)
        {
            winCondition = WinConditions.EvilWin;
            winConditionMet = true;
            Debug.Log("Win Condition Met: Spies Win");
        }
        else if (roundTimer <= 0)
        {
            winCondition = WinConditions.TimeOut;
            winConditionMet = true;
            Debug.Log("Win Condition Met: Time Ran Out");
        }

    }

    [Command(requiresAuthority = false)]
    public void CmdUpdateEvidence(int amount)
    {
        if (spiesRevealed)
            return;

        int newEvidenceValue = evidencePercent + amount;
        newEvidenceValue = Mathf.Clamp(newEvidenceValue, 0, 100);

        evidencePercent = newEvidenceValue;

        if (evidencePercent == 100 && !spiesRevealed)
        {
            spiesRevealed = true;
            RevealSpies();
        }
    }

    void OnEvidenceChanged(int oldValue, int newValue)
    {
        if (evidenceText != null)
        {
            if (newValue == 100)
            {
                evidenceText.text = "SPIES REVEALED!";
                evidenceImage.fillAmount = 1;
            }
            else
            {
                evidenceText.text = "Evidence Gathered: " + evidencePercent.ToString() + "%";

                evidenceImage.fillAmount = (float)evidencePercent / 100;
            }
        }
    }

    void RevealSpies()
    {
        if (roundSpies.Count == 1)
        {
            spyInformation = "The spy is " + ColorWord(roundSpies[0].playerName, roundSpies[0].coatTrimColour);
        }
        else if (roundSpies.Count == 2)
        {
            spyInformation = ColorWord(roundSpies[0].playerName, roundSpies[0].coatTrimColour) + " and " + ColorWord(roundSpies[1].playerName, roundSpies[1].coatTrimColour) + " are spies!";
        }
        else if (roundSpies.Count == 3)
        {
            spyInformation = ColorWord(roundSpies[0].playerName, roundSpies[0].coatTrimColour) + ", " + ColorWord(roundSpies[1].playerName, roundSpies[1].coatTrimColour) + " and " + ColorWord(roundSpies[2].playerName, roundSpies[2].coatTrimColour) + " are spies!";
        }
    }
    void OnSpyInformationChanged(string oldValue, string newValue)
    {
        revealedSpiesText.text = newValue;
    }

    public static string ColorWord(string text, Color color)
    {
        return "<color=#" + ColorUtility.ToHtmlStringRGBA(color) + ">" + text + "</color>";
    }

}

[System.Serializable]
public struct AgentData
{
    public string agentName;
    public Color agentColour;
    public Color agentSecondaryColour;
}

[System.Serializable]
public class PlayerSettings
{
    public int playerCount;
    public int spyCount;

    public int itemsToSpawn;
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