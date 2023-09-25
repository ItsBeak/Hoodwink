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
    public Color[] coatColours, trimColours, pantsColours, shoesColours;
    List<AgentData> allAgents;
    List<AgentData> availableAgents;

    [Header("Default Sidearm Settings")]
    public GameObject defaultClientSidearm;
    public GameObject defaultObserverSidearm;

    [Header("Default Sidearm Settings")]
    public GameObject defaultClientHolstered;
    public GameObject defaultObserverHolstered;

    [Header("Gadgets")]
    public List<GameObject> spyGadgets;
    public List<GameObject> agentGadgets;

    [Header("Map Pool")]
    [Scene] public string[] maps;

    [Header("Round Timer Settings")]
    public float warmupLength;
    [SyncVar] float warmupTimer;
    public float roundLength;
    [SyncVar] float roundTimer;
    public float postGameLength;
    [SyncVar] float postGameTimer;

    [Header("Player Intro Settings")]
    public PlayableDirector introTimeline;
    public PlayableAsset[] intros;
    public GameObject introCamera;
    public Transform hatAnchor;
    public TextMeshProUGUI introPlayerName, introPlayerAgentName;
    public Renderer playerRenderer, coatRenderer, coatTrimRenderer;

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

                    foreach (AgentData agent in allAgents)
                    {
                        availableAgents.Add(agent);
                    }

                    foreach (var player in roundPlayers)
                    {
                        int randomAgent = Random.Range(0, availableAgents.Count);

                        player.agentName = availableAgents[randomAgent].agentName;
                        player.coatColour = availableAgents[randomAgent].agentSecondaryColour;
                        player.coatTrimColour = availableAgents[randomAgent].agentColour;

                        player.pantsColour = pantsColours[Random.Range(0, pantsColours.Length)];
                        player.shoesColour = shoesColours[Random.Range(0, shoesColours.Length)];

                        availableAgents.RemoveAt(randomAgent);
                    }

                    List<int> introIndexes = new List<int>();

                    for (int i = 0; i < intros.Length; i++)
                    {
                        introIndexes.Add(i);
                    }

                    List<IntroCosmeticData> players = new List<IntroCosmeticData>();

                    foreach (var player in roundPlayers)
                    {
                        IntroCosmeticData newPlayer = new IntroCosmeticData();

                        newPlayer.playerName = player.playerName;
                        newPlayer.agentName = player.agentName;
                        newPlayer.agentColour = player.coatTrimColour;
                        newPlayer.agentSecondaryColour = player.coatColour;
                        newPlayer.agentHatIndex = player.hatIndex;

                        int newIntroIndex = introIndexes[Random.Range(0, introIndexes.Count)];
                        newPlayer.introIndex = newIntroIndex;
                        introIndexes.Remove(newIntroIndex);

                        players.Add(newPlayer);
                    }

                    CmdPlayIntro(players);
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
                    roundEndManager.RpcResetUI();
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

        player.agentName = "Anonymous";

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
        }

        foreach (var player in roundPlayers)
        {

            int randomSpawn = Random.Range(0, lobbySpawns.Length);

            player.TeleportPlayer(lobbySpawns[randomSpawn].position, lobbySpawns[randomSpawn].rotation);

            player.agentName = "Anonymous";
            player.coatColour = coatColours[Random.Range(0, coatColours.Length)];
            player.coatTrimColour = trimColours[Random.Range(0, trimColours.Length)];

            NetworkServer.Destroy(player.equipment.firstGadget.gameObject);
            NetworkServer.Destroy(player.equipment.secondGadget.gameObject);

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

        agentsLeft = 0;
        spiesLeft = 0;

        evidencePercent = 0;
        spiesRevealed = false;
        introStarted = false;

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

                List<GameObject> gadgetPool = new List<GameObject>();

                foreach (GameObject gadget in agentGadgets)
                {
                    gadgetPool.Add(gadget);
                }

                int gadgetIndex = Random.Range(0, gadgetPool.Count);

                GameObject firstGadget = Instantiate(gadgetPool[gadgetIndex]);
                NetworkServer.Spawn(firstGadget, roundPlayers[i].connectionToClient);
                roundPlayers[i].equipment.RpcEquipFirstGadget(firstGadget);
                gadgetPool.Remove(gadgetPool[gadgetIndex]);

                gadgetIndex = Random.Range(0, gadgetPool.Count);

                GameObject secondGadget = Instantiate(gadgetPool[gadgetIndex]);
                NetworkServer.Spawn(secondGadget, roundPlayers[i].connectionToClient);
                roundPlayers[i].equipment.RpcEquipSecondGadget(secondGadget);
                gadgetPool.Remove(gadgetPool[gadgetIndex]);

                gadgetPool.Clear();

            }

            GameObject newClientSidearm = Instantiate(defaultClientSidearm);
            GameObject newObserverSidearm = Instantiate(defaultObserverSidearm);
            NetworkServer.Spawn(newClientSidearm, roundPlayers[i].connectionToClient);
            NetworkServer.Spawn(newObserverSidearm, roundPlayers[i].connectionToClient);
            roundPlayers[i].equipment.RpcEquipSidearm(newClientSidearm, newObserverSidearm);

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
        {
            SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
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

        if (agentsLeft == 0 && spiesLeft == 0)
        {
            winCondition = WinConditions.Draw;
            winConditionMet = true;
            Debug.Log("Win Condition Met: Draw");
        }
        else if (spiesLeft == 0)
        {
            winCondition = WinConditions.GoodWin;
            winConditionMet = true;
            Debug.Log("Win Condition Met: Agents Win");

            RoundEndData[] endData = new RoundEndData[roundSpies.Count];

            for (int i = 0; i < endData.Length; i++)
            {
                endData[i].agentData.agentName = roundSpies[i].agentName;
                endData[i].agentData.agentColour = roundSpies[i].coatTrimColour;
                endData[i].agentData.agentSecondaryColour = roundSpies[i].coatColour;
                endData[i].isDead = roundDeadPlayers.Contains(roundSpies[i]);
            }

            roundEndManager.RpcSetAgentCards(endData, "Spies Eliminated");
        }
        else if (agentsLeft == 0)
        {
            winCondition = WinConditions.EvilWin;
            winConditionMet = true;
            Debug.Log("Win Condition Met: Spies Win");

            RoundEndData[] endData = new RoundEndData[roundAgents.Count];

            for (int i = 0; i < endData.Length; i++)
            {
                endData[i].agentData.agentName = roundAgents[i].agentName;
                endData[i].agentData.agentColour = roundAgents[i].coatTrimColour;
                endData[i].agentData.agentSecondaryColour = roundAgents[i].coatColour;
                endData[i].isDead = roundDeadPlayers.Contains(roundAgents[i]);
            }

            roundEndManager.RpcSetAgentCards(endData, "Agents Eliminated");

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
            spyInformation = "The spy is " + ColorWord(roundSpies[0].agentName, roundSpies[0].coatTrimColour);
        }
        else if (roundSpies.Count == 2)
        {
            spyInformation = ColorWord(roundSpies[0].agentName, roundSpies[0].coatTrimColour) + " and " + ColorWord(roundSpies[1].agentName, roundSpies[1].coatTrimColour) + " are spies!";
        }
        else if (roundSpies.Count == 3)
        {
            spyInformation = ColorWord(roundSpies[0].agentName, roundSpies[0].coatTrimColour) + ", " + ColorWord(roundSpies[1].agentName, roundSpies[1].coatTrimColour) + " and " + ColorWord(roundSpies[2].agentName, roundSpies[2].coatTrimColour) + " are spies!";
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

    IEnumerator PlayIntroCutscene(List<IntroCosmeticData> players)
    {
        yield return new WaitForSeconds(0.25f);

        introCamera.SetActive(true);
        H_TransitionManager.instance.SetClear();
        playerUIGroup.alpha = 0;

        foreach (var player in players)
        {

            //set cosmetics

            playerRenderer.material.SetColor("_ShirtColour", Color.clear);
            coatRenderer.material.color = player.agentSecondaryColour;
            coatTrimRenderer.material.color = player.agentColour;

            introPlayerName.text = player.playerName;
            introPlayerAgentName.text = "as " + ColorWord("Agent " + player.agentName, player.agentColour);

            Instantiate(H_CosmeticManager.instance.hats[player.agentHatIndex].cosmeticPrefab, hatAnchor);

            introTimeline.playableAsset = intros[player.introIndex];

            introTimeline.time = 0;
            introTimeline.Play();

            yield return new WaitForSeconds((float)introTimeline.playableAsset.duration);

            // clear hat

            foreach (Transform child in hatAnchor.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            yield return new WaitForSeconds(0.5f);

        }

        yield return new WaitForSeconds(0.25f);

        introCamera.SetActive(false);
        playerUIGroup.alpha = 1;
        H_TransitionManager.instance.SetBlack();
        H_TransitionManager.instance.FadeOut(0.5f);

        if (isServer)
        {
            currentRoundStage = RoundStage.Game;

            foreach (var player in roundPlayers)
            {
                player.isHudHidden = false;
            }
        }

    }

    [Command(requiresAuthority = false)]
    void CmdPlayIntro(List<IntroCosmeticData> players)
    {
        RpcPlayIntro(players);

        foreach (var player in roundPlayers)
        {
            player.isHudHidden = true;
        }
    }

    [ClientRpc]
    void RpcPlayIntro(List<IntroCosmeticData> players)
    {
        StartCoroutine(PlayIntroCutscene(players));

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

    void UpdateDisplayPlayer(IntroCosmeticData player)
    {

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
public struct IntroCosmeticData
{
    public string playerName;
    public string agentName;
    public Color agentColour;
    public Color agentSecondaryColour;
    public int agentHatIndex;
    public int introIndex;
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
    Intro,
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