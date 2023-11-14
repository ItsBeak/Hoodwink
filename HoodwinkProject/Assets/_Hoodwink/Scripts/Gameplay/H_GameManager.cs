using UnityEngine;
using Mirror;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Playables;
using System;
using Random = UnityEngine.Random;
using System.Threading;
using OdinNative.Unity.Samples;

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
    [SyncVar] bool evidenceCompleted = false;
    bool gameStarted = false;

    [HideInInspector] public List<H_PlayerBrain> serverPlayers;
    [HideInInspector] public List<H_PlayerBrain> roundPlayers;
    [HideInInspector] public List<H_PlayerBrain> roundDeadPlayers;
    [HideInInspector] public List<H_PlayerBrain> roundAgents;
    [HideInInspector] public List<H_PlayerBrain> roundSpies;
    int agentsLeft = 0;
    int spiesLeft = 0;

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
    public Color[] jacketColours, vestColours, pantsColours, tieColours;
    List<AgentData> allAgents;
    List<AgentData> availableAgents;

    [Header("Default Sidearm Settings")]
    public GameObject defaultClientSidearm;
    public GameObject defaultObserverSidearm;

    [Header("Gadgets")]
    public List<GameObject> spyGadgets;
    //public List<GameObject> agentGadgets;

    [Header("Map Pool")]
    [Scene] public string[] maps;

    [Header("Round Timer Settings")]
    public float warmupLength;
    [SyncVar] float warmupTimer;
    public float roundLength;
    [SyncVar] float roundTimer;
    public float postGameLength;
    [SyncVar] float postGameTimer;

    [Header("Phone Settings")]
    public float minimumPhoneTime;
    public float maximumPhoneTime;
    float phoneTimer;
    bool isPhoneActive;
    H_Phone[] allPhones;

    [Header("Components")]
    public TextMeshProUGUI pingDisplay;
    public TextMeshProUGUI timerDisplay;
    public CanvasGroup playerUIGroup;
    H_ObjectManager objectManager;
    H_RoundEndManager roundEndManager;

    [Header("Debugging")]
    public bool enableDebugLogs;
    public bool overrideMinimumPlayerCount;
    public bool allowServerToSkipGame;
    public bool allowServerToForceStart;
    public bool allowDebugOverrideKeys;
    public PlayerSettings overrideSettings;
    PlayerSettings currentSettings;

    bool introStarted;

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
        roundEndManager = GetComponent<H_RoundEndManager>();
        revealedSpiesText.text = "";

        FindObjectOfType<SimplePushToTalk>().showHUD = true;
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

                if (Input.GetKeyDown(KeyCode.Insert) && allowServerToForceStart)
                {
                    StartRound();
                    return;
                }

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
                    AssignRoles();

                    currentRoundStage = RoundStage.Intro;
                }

                break;
            #endregion

            #region Intro Update
            case RoundStage.Intro:

                timerDisplay.text = "Game starting...";

                if (!isServer)
                    return;

                if (!introStarted)
                {
                    introStarted = true;

                    allPhones = GameObject.FindObjectsOfType<H_Phone>();

                    phoneTimer = Random.Range(minimumPhoneTime, maximumPhoneTime);
                    isPhoneActive = false;

                    foreach (var player in roundPlayers)
                    {
                        player.isHudHidden = true;

                        if (player.currentAlignment == AgentAlignment.Agent)
                        {
                            IntroCosmeticData playerData = new IntroCosmeticData();

                            playerData.playerName = player.playerName;
                            playerData.agentData = player.agentData;

                            player.RpcPlayAgentIntro(playerData);
                            Debug.Log("Playing agent intro on player: " + player.playerName);

                        }
                        else if (player.currentAlignment == AgentAlignment.Spy)
                        {
                            List<IntroCosmeticData> playerData = new List<IntroCosmeticData>();

                            IntroCosmeticData localPlayer = new IntroCosmeticData();

                            localPlayer.playerName = player.playerName;
                            localPlayer.agentData = player.agentData;

                            playerData.Add(localPlayer);

                            foreach (var otherSpy in roundSpies)
                            {
                                if (otherSpy != player)
                                {
                                    IntroCosmeticData otherSpyData = new IntroCosmeticData();

                                    otherSpyData.playerName = otherSpy.playerName;
                                    otherSpyData.agentData = otherSpy.agentData;

                                    playerData.Add(otherSpyData);
                                }
                            }

                            player.RpcPlaySpyIntro(playerData);
                            Debug.Log("Playing spy intro on player: " + player.playerName);
                        }
                    }
                }

                break;
            #endregion

            #region Game Stage Update
            case RoundStage.Game:

                if (!gameStarted)
                {
                    gameStarted = true;
                    foreach (var player in roundPlayers)
                    {
                        player.equipment.RpcSetBusy(false);
                    }
                }

                timerDisplay.text = "In game, time left: " + Mathf.RoundToInt(roundTimer + 0.5f).ToString();

                if (!isServer)
                    return;

                roundTimer -= 1 * Time.deltaTime;

                bool phoneActive = false;

                if (allPhones.Length == 0)
                {
                    allPhones = GameObject.FindObjectsOfType<H_Phone>();

                    phoneTimer = Random.Range(minimumPhoneTime, maximumPhoneTime);
                    isPhoneActive = false;
                }

                foreach (var phone in allPhones)
                {
                    if (phone.isActive)
                    {
                        phoneActive = true;
                    }
                }

                isPhoneActive = phoneActive;

                if (!isPhoneActive)
                {
                    phoneTimer -= 1 * Time.deltaTime;

                    if (phoneTimer <= 0)
                    {
                        allPhones[Random.Range(0, allPhones.Length)].Ring();

                        isPhoneActive=true;

                        phoneTimer = Random.Range(minimumPhoneTime, maximumPhoneTime);

                        Debug.Log("Ringing new phone");
                    }
                }

                if (Input.GetKeyDown(KeyCode.Backspace) && allowServerToSkipGame)
                {
                    roundTimer -= 999;
                }

                if (winConditionMet)
                {
                    currentRoundStage = RoundStage.PostGame;
                }

                if (roundTimer <= 0)
                {
                    CheckWinConditions();
                    currentRoundStage = RoundStage.PostGame;
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
                    EndRound();
                    ResetRoles();
                    ResetPlayerStates();
                    roundEndManager.RpcResetUI();
                }

                break;
            #endregion

            default: break;
        }

        if (Input.GetKeyDown(KeyCode.L) && allowDebugOverrideKeys)
        {
            CmdUpdateEvidence(10);
        }
        else if (Input.GetKeyDown(KeyCode.K) && allowDebugOverrideKeys)
        {
            CmdUpdateEvidence(-10);
        }
        else if (Input.GetKeyDown(KeyCode.M) && allowDebugOverrideKeys)
        {
            allPhones[Random.Range(0, allPhones.Length)].Ring();
        }

    }

    [Command(requiresAuthority = false)]
    public void CmdRegisterPlayer(H_PlayerBrain player)
    {
        serverPlayers.Add(player);

        ResetPlayerColours(player);
    }

    [Server]
    public void SetPlayerData(H_PlayerBrain player, AgentData data)
    {
        AgentData newData = new AgentData();

        newData.agentName = data.agentName;

        newData.primaryColour = data.primaryColour;
        newData.secondaryColour = data.secondaryColour;
        newData.pantsColour = data.pantsColour;
        newData.vestColour = data.vestColour;
        newData.tieColour = data.tieColour;

        newData.hatIndex = player.agentData.hatIndex;
        newData.vestIndex = player.agentData.vestIndex;
        newData.suitIndex = player.agentData.suitIndex;

        player.agentData = newData;
    }

    [Server]
    public void ResetPlayerColours(H_PlayerBrain player)
    {
        AgentData newData = new AgentData();

        newData.agentName = "Anonymous";

        newData.primaryColour = jacketColours[Random.Range(0, jacketColours.Length)];
        newData.secondaryColour = vestColours[Random.Range(0, vestColours.Length)];
        newData.pantsColour = pantsColours[Random.Range(0, pantsColours.Length)];
        newData.vestColour = vestColours[Random.Range(0, vestColours.Length)];
        newData.tieColour = tieColours[Random.Range(0, tieColours.Length)];

        newData.hatIndex = player.agentData.hatIndex;
        newData.vestIndex = player.agentData.vestIndex;
        newData.suitIndex = player.agentData.suitIndex;

        player.agentData = newData;
    }
    


    [Command(requiresAuthority = false)]
    public void CmdUnregisterPlayer(H_PlayerBrain player)
    {
        serverPlayers.Remove(player);
        roundPlayers.Remove(player);
        roundAgents.Remove(player);
        roundSpies.Remove(player);
        roundDeadPlayers.Remove(player);

        player.equipment.RpcTryDropItem();

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

        CmdFadeIn(0.3f);

        currentRoundStage = RoundStage.Warmup;

    }

    [Server]
    public void EndRound()
    {
        foreach (var player in serverPlayers)
        {
            player.isReady = false;
            player.equipment.RpcTryDropItem();
            player.equipment.RpcSetBusy(false);
        }

        foreach (var player in roundPlayers)
        {

            int randomSpawn = Random.Range(0, lobbySpawns.Length);

            player.TeleportPlayer(lobbySpawns[randomSpawn].position, lobbySpawns[randomSpawn].rotation);

            ResetPlayerColours(player);

            if (player.currentAlignment == AgentAlignment.Spy)
            {
                NetworkServer.Destroy(player.equipment.firstGadget.gameObject);
                NetworkServer.Destroy(player.equipment.secondGadget.gameObject);
            }

            NetworkServer.Destroy(player.equipment.sidearmClientObject.gameObject);
            NetworkServer.Destroy(player.equipment.sidearmObserverObject.gameObject);

            player.equipment.RpcClearSidearmSlot();
        }

        roundPlayers.Clear();
        roundAgents.Clear();
        roundSpies.Clear();
        roundDeadPlayers.Clear();

        winConditionMet = false;

        availableAgents.Clear();

        agentsLeft = 0;
        spiesLeft = 0;

        evidencePercent = 0;
        spiesRevealed = false;
        introStarted = false;
        evidenceCompleted = false;

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
        Debug.Log("Loading Level: '" + chosenScene + "' out of " + maps.Length + " maps.");


        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(chosenScene, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            if (enableDebugLogs)
                Debug.LogWarning("Loading Level: " + chosenScene);
            yield return null;
        }

        if (enableDebugLogs)
            Debug.LogWarning("Level Loaded: " + chosenScene);

        LightProbes.Tetrahedralize();

        RpcLoadMap(chosenScene);

        yield return new WaitForSeconds(5);

        currentLevel = FindObjectOfType<H_LevelData>();

        Transform[] spawns = currentLevel.playerSpawnPoints;

        foreach (var player in roundPlayers)
        {
            player.equipment.RpcTryDropItem();

            int randomSpawn = Random.Range(0, spawns.Length);

            player.TeleportPlayer(spawns[randomSpawn].position, spawns[randomSpawn].rotation);

            player.equipment.currentSlot = EquipmentSlot.PrimaryItem;
            player.equipment.RpcSetPrimary();
        }



        Debug.Log("Spawning objects");

        objectManager.SpawnObjects(currentSettings);

    }

    [Server]
    private void AssignRoles()
    {
        foreach (AgentData agent in allAgents)
        {
            availableAgents.Add(agent);
        }

        foreach (var player in roundPlayers)
        {
            int randomAgent = Random.Range(0, availableAgents.Count);

            SetPlayerData(player, availableAgents[randomAgent]);

            availableAgents.RemoveAt(randomAgent);

            player.equipment.currentSlot = EquipmentSlot.PrimaryItem;
            player.RpcSetCanMove(false);
            player.equipment.RpcSetBusy(true);
        }

        roundSpiesRemaining = currentSettings.spyCount;

        while (roundSpiesRemaining > 0)
        {
            int randomPlayerIndex = Random.Range(0, roundPlayers.Count);

            if (roundPlayers[randomPlayerIndex].currentAlignment != AgentAlignment.Spy)
            {
                roundPlayers[randomPlayerIndex].currentAlignment = AgentAlignment.Spy;
                roundSpies.Add(roundPlayers[randomPlayerIndex]);
                roundSpiesRemaining--;
                spiesLeft++;

                List<GameObject> gadgetPool = new List<GameObject>();

                foreach (GameObject gadget in spyGadgets)
                {
                    gadgetPool.Add(gadget);
                }

                int gadgetIndex = Random.Range(0, gadgetPool.Count);

                GameObject firstGadget = Instantiate(gadgetPool[gadgetIndex]);
                NetworkServer.Spawn(firstGadget, roundPlayers[randomPlayerIndex].connectionToClient);
                roundPlayers[randomPlayerIndex].equipment.RpcEquipFirstGadget(firstGadget);
                gadgetPool.Remove(gadgetPool[gadgetIndex]);

                gadgetIndex = Random.Range(0, gadgetPool.Count);

                GameObject secondGadget = Instantiate(gadgetPool[gadgetIndex]);
                NetworkServer.Spawn(secondGadget, roundPlayers[randomPlayerIndex].connectionToClient);
                roundPlayers[randomPlayerIndex].equipment.RpcEquipSecondGadget(secondGadget);
                gadgetPool.Remove(gadgetPool[gadgetIndex]);

                gadgetPool.Clear();
            }
        }

        for (int i = 0; i < roundPlayers.Count; i++)
        {
            if (roundPlayers[i].currentAlignment != AgentAlignment.Spy)
            {
                roundPlayers[i].currentAlignment = AgentAlignment.Agent;
                roundAgents.Add(roundPlayers[i]);
                agentsLeft++;

                //List<GameObject> gadgetPool = new List<GameObject>();
                //
                //foreach (GameObject gadget in agentGadgets)
                //{
                //    gadgetPool.Add(gadget);
                //}
                //
                //int gadgetIndex = Random.Range(0, gadgetPool.Count);
                //
                //GameObject firstGadget = Instantiate(gadgetPool[gadgetIndex]);
                //NetworkServer.Spawn(firstGadget, roundPlayers[i].connectionToClient);
                //roundPlayers[i].equipment.RpcEquipFirstGadget(firstGadget);
                //gadgetPool.Remove(gadgetPool[gadgetIndex]);
                //
                //gadgetIndex = Random.Range(0, gadgetPool.Count);
                //
                //GameObject secondGadget = Instantiate(gadgetPool[gadgetIndex]);
                //NetworkServer.Spawn(secondGadget, roundPlayers[i].connectionToClient);
                //roundPlayers[i].equipment.RpcEquipSecondGadget(secondGadget);
                //gadgetPool.Remove(gadgetPool[gadgetIndex]);
                //
                //gadgetPool.Clear();

            }

            GameObject newClientSidearm = Instantiate(defaultClientSidearm);
            GameObject newObserverSidearm = Instantiate(defaultObserverSidearm);
            NetworkServer.Spawn(newClientSidearm, roundPlayers[i].connectionToClient);
            NetworkServer.Spawn(newObserverSidearm, roundPlayers[i].connectionToClient);
            roundPlayers[i].equipment.RpcEquipSidearm(newClientSidearm, newObserverSidearm);
        }



    }

    [ClientRpc]
    void RpcLoadMap(string sceneName)
    {
        StartCoroutine(LoadMap(sceneName));
    }

    IEnumerator LoadMap(string sceneName)
    {
        if (!isServer)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            LightProbes.Tetrahedralize();
        }
    }

    [ClientRpc]
    void RpcUnloadMap(string scene)
    {
        if (SceneManager.GetSceneByPath(scene).isLoaded)
        {
            SceneManager.UnloadSceneAsync(scene);
        }
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
                agentsLeft -= 1;
                roundDeadPlayers.Add(player);
            }
            else if (player.currentAlignment == AgentAlignment.Spy)
            {
                spiesLeft -= 1;
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

        if (agentsLeft == 0 && spiesLeft == 0)            // draw
        {
            winCondition = WinConditions.Draw;
            winConditionMet = true;

            Debug.Log("Win Condition Met: Draw");
        }
        else if (agentsLeft == 0)            // spies win - agents eliminated
        {
            winCondition = WinConditions.AgentsEliminated;
            winConditionMet = true;
            Debug.Log("Win Condition Met: Spies Win");

            RoundEndData[] endData = new RoundEndData[roundAgents.Count];

            for (int i = 0; i < endData.Length; i++)
            {
                endData[i].agentData.agentName = roundAgents[i].agentData.agentName;
                endData[i].agentData.primaryColour = roundAgents[i].agentData.primaryColour;
                endData[i].agentData.secondaryColour = roundAgents[i].agentData.secondaryColour;
                endData[i].isDead = roundDeadPlayers.Contains(roundAgents[i]);
            }

            roundEndManager.RpcSetAgentCards(endData, "Agents Eliminated");
        }
        else if (spiesLeft == 0)            // agents win - spies eliminated
        {
            winCondition = WinConditions.SpiesEliminated;
            winConditionMet = true;

            Debug.Log("Win Condition Met: Agents Win");

            RoundEndData[] endData = new RoundEndData[roundSpies.Count];

            for (int i = 0; i < endData.Length; i++)
            {
                endData[i].agentData.agentName = roundSpies[i].agentData.agentName;
                endData[i].agentData.primaryColour = roundSpies[i].agentData.primaryColour;
                endData[i].agentData.secondaryColour = roundSpies[i].agentData.secondaryColour;
                endData[i].isDead = roundDeadPlayers.Contains(roundSpies[i]);
            }

            roundEndManager.RpcSetAgentCards(endData, "Spies Eliminated");
        }
        else if (evidenceCompleted)            // agents win - evidence gathered
        {
            winCondition = WinConditions.EvidenceCompleted;
            winConditionMet = true;
            Debug.Log("Win Condition Met: Evidence Completed");
        }
        else if (roundTimer <= 0)            // spies win - time limit reached
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
                if (!spiesRevealed)
                {
                    evidenceText.text = "Evidence Gathered: " + evidencePercent.ToString() + "%";
                }
                else
                {
                    evidenceText.text = "Evidence Transferred: " + (100 - evidencePercent).ToString() + "%";
                }

                evidenceImage.fillAmount = (float)evidencePercent / 100;

            }
        }
    }

    void RevealSpies()
    {
        if (roundSpies.Count == 1)
        {
            spyInformation = "The spy is " + ColorWord(roundSpies[0].agentData.agentName, roundSpies[0].agentData.primaryColour);
        }
        else if (roundSpies.Count == 2)
        {
            spyInformation = ColorWord(roundSpies[0].agentData.agentName, roundSpies[0].agentData.primaryColour) + " and " + ColorWord(roundSpies[1].agentData.agentName, roundSpies[1].agentData.primaryColour) + " are spies!";
        }
        else if (roundSpies.Count == 3)
        {
            spyInformation = ColorWord(roundSpies[0].agentData.agentName, roundSpies[0].agentData.primaryColour) + ", " + ColorWord(roundSpies[1].agentData.agentName, roundSpies[1].agentData.primaryColour) + " and " + ColorWord(roundSpies[2].agentData.agentName, roundSpies[2].agentData.primaryColour) + " are spies!";
        }

        StartCoroutine(DrainEvidence());

    }

    void OnSpyInformationChanged(string oldValue, string newValue)
    {
        revealedSpiesText.text = newValue;
    }

    IEnumerator DrainEvidence()
    {
        Debug.Log("Started draining evidence");

        yield return new WaitForSeconds(5);

        while (evidencePercent > 0)
        {
            evidencePercent--;
            yield return new WaitForSeconds(0.316f);
        }

        Debug.Log("Evidence drained");
        evidenceCompleted = true;

        CheckWinConditions();

    }

    public static string ColorWord(string text, Color color)
    {
        return "<color=#" + ColorUtility.ToHtmlStringRGBA(color) + ">" + text + "</color>";
    }

    [Command(requiresAuthority = false)]
    void CmdFadeIn(float speed)
    {
        RpcFadeIn(speed);
    }

    [ClientRpc]
    void RpcFadeIn(float speed)
    {
        H_TransitionManager.instance.FadeIn(speed);
    }

    [Command(requiresAuthority = false)]
    void CmdFadeOut(float speed)
    {
        RpcFadeOut(speed);
    }

    [ClientRpc]
    void RpcFadeOut(float speed)
    {
        H_TransitionManager.instance.FadeOut(speed);
    }
}

[System.Serializable]
public struct AgentData
{
    public string agentName;

    public Color primaryColour;
    public Color secondaryColour;
    public Color pantsColour, vestColour, tieColour;

    public int hatIndex;
    public int suitIndex;
    public int vestIndex;
}

[System.Serializable]
public class PlayerSettings
{
    public int playerCount;
    public int spyCount;

    public int itemsToSpawn;
    public int phonesToSpawn = 2;
}

public enum RoundStage
{
    Lobby,
    Warmup,
    Intro,
    Game,
    PostGame,
}

public enum WinConditions
{
    AgentsEliminated,
    SpiesEliminated,
    EvidenceCompleted,
    TimeOut,
    Draw
}