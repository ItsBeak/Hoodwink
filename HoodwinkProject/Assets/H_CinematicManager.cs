using Mirror;
using SteamAudio;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class H_CinematicManager : NetworkBehaviour
{
    public static H_CinematicManager instance { get; private set; }

    [Header("Intro Timelines")]
    public PlayableDirector agentIntroTimeline;
    public PlayableDirector spyIntroTimeline;

    [Header("Round End Timelines")]
    public PlayableDirector agentsEliminatedTimeline;
    public PlayableDirector spiesEliminatedTimeline;
    public PlayableDirector evidenceGatheredTimeline;
    public PlayableDirector timeOutTimeline;

    [Header("Agent Intro")]
    public H_CosmeticDisplay agentDisplay;
    public TextMeshProUGUI agentPlayerName, agentPlayerAgentName, agentPlayerRole;

    [Header("Spy Intro")]
    public H_CosmeticDisplay spyDisplay;
    public H_CosmeticDisplay firstSpyTeammateDisplay, secondSpyTeammateDisplay;
    public TextMeshProUGUI spyPlayerName, spyPlayerAgentName, spyPlayerRole;
    public TextMeshProUGUI firstSpyPlayerName, firstSpyPlayerAgentName;
    public TextMeshProUGUI secondSpyPlayerName, secondSpyPlayerAgentName;

    [Header("Agents Eliminated Outro")]
    public H_CosmeticDisplay firstOutroSpyDisplay;
    public H_CosmeticDisplay secondOutroSpyDisplay, thirdOutroSpyDisplay;
    public TextMeshProUGUI firstOutroSpyName, secondOutroSpyName, thirdOutroSpyName;

    [Header("Agents Eliminated Outro")]
    public H_CosmeticDisplay firstOutroAgentDisplay;
    public H_CosmeticDisplay secondOutroAgentDisplay, thirdOutroAgentDisplay, fourthOutroAgentDisplay, fifthOutroAgentDisplay;
    public TextMeshProUGUI firstOutroAgentName, secondOutroAgentName, thirdOutroAgentName, fourthOutroAgentName, fifthOutroAgentName;


    [Header("Evidence Gathered Outro")]
    public H_CosmeticDisplay evidenceAgentDisplay;

    H_RendererProbeTool probes;

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

    private void Start()
    {
        probes = GetComponent<H_RendererProbeTool>();
    }

    public void PlayAgentIntro(IntroCosmeticData player)
    {
        StartCoroutine(PlayAgentIntroCutscene(player));
    }

    public void PlaySpyIntro(List<IntroCosmeticData> players)
    {
        StartCoroutine(PlaySpyIntroCutscene(players));
    }

    [ClientRpc]
    public void PlayAgentsEliminatedOutro(List<IntroCosmeticData> spies)
    {
        StartCoroutine(PlayAgentsEliminatedCutscene(spies));
    }

    [ClientRpc]
    public void PlaySpiesEliminatedOutro(List<IntroCosmeticData> agents)
    {
        StartCoroutine(PlaySpiesEliminatedCutscene(agents));
    }

    [ClientRpc]
    public void PlayEvidenceGatheredOutro(IntroCosmeticData agent)
    {
        StartCoroutine(PlayEvidenceGatheredCutscene(agent));
    }

    [ClientRpc]
    public void PlayTimeOutOutro()
    {
        StartCoroutine(PlayTimeOutCutscene());
    }

    IEnumerator PlayAgentIntroCutscene(IntroCosmeticData player)
    {
        H_GameManager.instance.playerUIGroup.alpha = 0;

        ColourData playerColours = new ColourData();

        playerColours.primaryColour = player.agentData.primaryColour;
        playerColours.secondaryColour = player.agentData.secondaryColour;
        playerColours.pantsColour= player.agentData.pantsColour;
        playerColours.vestColour = player.agentData.vestColour;
        playerColours.tieColour= player.agentData.tieColour;

        agentDisplay.SetColour(playerColours);
        agentDisplay.SetHat(player.agentData.hatIndex);
        agentDisplay.ToggleSuit(player.agentData.suitIndex);
        agentDisplay.ToggleVest(player.agentData.vestIndex);

        agentPlayerName.text = player.playerName;
        agentPlayerAgentName.text = "Agent " + H_GameManager.ColorWord(player.agentData.agentName, player.agentData.primaryColour);
        agentPlayerRole.text = "You are an " + H_GameManager.ColorWord("Agent", Color.green);

        probes.DisableProbes();

        agentIntroTimeline.Play();
        H_TransitionManager.instance.SetClear();

        yield return new WaitForSeconds((float)agentIntroTimeline.playableAsset.duration);

        agentDisplay.ClearHat();

        H_GameManager.instance.playerUIGroup.alpha = 1;
        H_TransitionManager.instance.SetBlack();
        H_TransitionManager.instance.FadeOut(0.5f);

        if (isServer)
        {
            H_GameManager.instance.currentRoundStage = RoundStage.Game;

            foreach (var p in H_GameManager.instance.roundPlayers)
            {
                p.isHudHidden = false;
                p.RpcSetCanMove(true);
                p.equipment.SetBusy(false);
            }

            H_GameManager.instance.globalHideHud = false;

        }

    }

    IEnumerator PlaySpyIntroCutscene(List<IntroCosmeticData> players)
    {
        H_GameManager.instance.playerUIGroup.alpha = 0;

        ColourData playerColours = new ColourData();

        playerColours.primaryColour = players[0].agentData.primaryColour;
        playerColours.secondaryColour = players[0].agentData.secondaryColour;
        playerColours.pantsColour = players[0].agentData.pantsColour;
        playerColours.vestColour = players[0].agentData.vestColour;
        playerColours.tieColour = players[0].agentData.tieColour;

        spyDisplay.SetColour(playerColours);
        spyDisplay.SetHat(players[0].agentData.hatIndex);
        spyDisplay.ToggleSuit(players[0].agentData.suitIndex);
        spyDisplay.ToggleVest(players[0].agentData.vestIndex);

        spyPlayerName.text = players[0].playerName;
        spyPlayerAgentName.text = "Agent " + H_GameManager.ColorWord(players[0].agentData.agentName, players[0].agentData.primaryColour);

        firstSpyTeammateDisplay.gameObject.SetActive(false);
        secondSpyTeammateDisplay.gameObject.SetActive(false);

        if (players.Count == 1)
        {
            spyPlayerRole.text = "You are the " + H_GameManager.ColorWord("Spy", Color.red);

            firstSpyPlayerName.text = "";
            firstSpyPlayerAgentName.text = "";

            secondSpyPlayerName.text = "";
            secondSpyPlayerAgentName.text = "";

        }
        else
        {
            spyPlayerRole.text = "You are " + H_GameManager.ColorWord("Spies", Color.red);

            if (players.Count == 2 || players.Count == 3)
            {
                firstSpyTeammateDisplay.gameObject.SetActive(true);

                ColourData firstTeammateColours = new ColourData();

                firstTeammateColours.primaryColour = players[1].agentData.primaryColour;
                firstTeammateColours.secondaryColour = players[1].agentData.secondaryColour;
                firstTeammateColours.pantsColour = players[1].agentData.pantsColour;
                firstTeammateColours.vestColour = players[1].agentData.vestColour;
                firstTeammateColours.tieColour = players[1].agentData.tieColour;

                firstSpyTeammateDisplay.SetColour(firstTeammateColours);
                firstSpyTeammateDisplay.SetHat(players[1].agentData.hatIndex);
                firstSpyTeammateDisplay.ToggleSuit(players[1].agentData.suitIndex);
                firstSpyTeammateDisplay.ToggleVest(players[1].agentData.vestIndex);

                firstSpyPlayerName.text = players[1].playerName;
                firstSpyPlayerAgentName.text = "Agent " + H_GameManager.ColorWord(players[1].agentData.agentName, players[1].agentData.primaryColour);
            }

            if (players.Count == 3)
            {
                secondSpyTeammateDisplay.gameObject.SetActive(true);

                ColourData secondTeammateColours = new ColourData();

                secondTeammateColours.primaryColour = players[2].agentData.primaryColour;
                secondTeammateColours.secondaryColour = players[2].agentData.secondaryColour;
                secondTeammateColours.pantsColour = players[2].agentData.pantsColour;
                secondTeammateColours.vestColour = players[2].agentData.vestColour;
                secondTeammateColours.tieColour = players[2].agentData.tieColour;

                secondSpyTeammateDisplay.SetColour(secondTeammateColours);
                secondSpyTeammateDisplay.SetHat(players[2].agentData.hatIndex);
                secondSpyTeammateDisplay.ToggleSuit(players[2].agentData.suitIndex);
                secondSpyTeammateDisplay.ToggleVest(players[2].agentData.vestIndex);

                secondSpyPlayerName.text = players[2].playerName;
                secondSpyPlayerAgentName.text = "Agent " + H_GameManager.ColorWord(players[2].agentData.agentName, players[2].agentData.primaryColour);
            }
        }

        probes.DisableProbes();

        spyIntroTimeline.Play();
        H_TransitionManager.instance.SetClear();

        yield return new WaitForSeconds((float)spyIntroTimeline.playableAsset.duration);

        spyDisplay.ClearHat();
        firstSpyTeammateDisplay.ClearHat();
        secondSpyTeammateDisplay.ClearHat();
        H_GameManager.instance.playerUIGroup.alpha = 1;
        H_TransitionManager.instance.SetBlack();
        H_TransitionManager.instance.FadeOut(0.5f);

        if (isServer)
        {
            H_GameManager.instance.currentRoundStage = RoundStage.Game;

            foreach (var p in H_GameManager.instance.roundPlayers)
            {
                p.isHudHidden = false;
                p.RpcSetCanMove(true);
                p.equipment.SetBusy(false);
            }

            H_GameManager.instance.globalHideHud = false;

        }

    }

    IEnumerator PlayAgentsEliminatedCutscene(List<IntroCosmeticData> spies)
    {
        H_GameManager.instance.playerUIGroup.alpha = 0;

        ColourData firstSpyColours = new ColourData();

        firstSpyColours.primaryColour = spies[0].agentData.primaryColour;
        firstSpyColours.secondaryColour = spies[0].agentData.secondaryColour;
        firstSpyColours.pantsColour = spies[0].agentData.pantsColour;
        firstSpyColours.vestColour = spies[0].agentData.vestColour;
        firstSpyColours.tieColour = spies[0].agentData.tieColour;

        firstOutroSpyDisplay.SetColour(firstSpyColours);
        firstOutroSpyDisplay.SetHat(spies[0].agentData.hatIndex);
        firstOutroSpyDisplay.ToggleSuit(spies[0].agentData.suitIndex);
        firstOutroSpyDisplay.ToggleVest(spies[0].agentData.vestIndex);
        firstOutroSpyName.text = spies[0].playerName;

        secondOutroSpyDisplay.gameObject.SetActive(false);
        thirdOutroSpyDisplay.gameObject.SetActive(false);

        if (spies.Count == 2 || spies.Count == 3)
        {
            secondOutroSpyDisplay.gameObject.SetActive(true);

            ColourData secondSpyColours = new ColourData();

            secondSpyColours.primaryColour = spies[1].agentData.primaryColour;
            secondSpyColours.secondaryColour = spies[1].agentData.secondaryColour;
            secondSpyColours.pantsColour = spies[1].agentData.pantsColour;
            secondSpyColours.vestColour = spies[1].agentData.vestColour;
            secondSpyColours.tieColour = spies[1].agentData.tieColour;

            secondOutroSpyDisplay.SetColour(secondSpyColours);
            secondOutroSpyDisplay.SetHat(spies[1].agentData.hatIndex);
            secondOutroSpyDisplay.ToggleSuit(spies[1].agentData.suitIndex);
            secondOutroSpyDisplay.ToggleVest(spies[1].agentData.vestIndex);

            secondOutroSpyName.text = spies[1].playerName;
        }

        if (spies.Count == 3)
        {
            thirdOutroSpyDisplay.gameObject.SetActive(true);

            ColourData thirdSpyColours = new ColourData();

            thirdSpyColours.primaryColour = spies[2].agentData.primaryColour;
            thirdSpyColours.secondaryColour = spies[2].agentData.secondaryColour;
            thirdSpyColours.pantsColour = spies[2].agentData.pantsColour;
            thirdSpyColours.vestColour = spies[2].agentData.vestColour;
            thirdSpyColours.tieColour = spies[2].agentData.tieColour;

            thirdOutroSpyDisplay.SetColour(thirdSpyColours);
            thirdOutroSpyDisplay.SetHat(spies[2].agentData.hatIndex);
            thirdOutroSpyDisplay.ToggleSuit(spies[2].agentData.suitIndex);
            thirdOutroSpyDisplay.ToggleVest(spies[2].agentData.vestIndex);

            thirdOutroSpyName.text = spies[2].playerName;
        }

        probes.DisableProbes();

        H_TransitionManager.instance.SetClear();
        H_TransitionManager.instance.FadeIn(1);

        yield return new WaitForSeconds(1.5f);

        agentsEliminatedTimeline.Play();
        H_TransitionManager.instance.SetClear();

        yield return new WaitForSeconds((float)agentsEliminatedTimeline.playableAsset.duration);

        firstOutroSpyDisplay.ClearHat();
        secondOutroSpyDisplay.ClearHat();
        thirdOutroSpyDisplay.ClearHat();

        firstOutroSpyName.text = "";
        secondOutroSpyName.text = "";
        thirdOutroSpyName.text = "";

        H_TransitionManager.instance.SetBlack();
        H_TransitionManager.instance.FadeOut(0.5f);
        H_GameManager.instance.playerUIGroup.alpha = 1;

        if (isServer)
        {
            foreach (var player in H_GameManager.instance.roundPlayers)
            {
                player.isHudHidden = false;
            }

            H_GameManager.instance.globalHideHud = false;

            H_GameManager.instance.RoundEnd();
        }

    }

    IEnumerator PlaySpiesEliminatedCutscene(List<IntroCosmeticData> agents)
    {
        H_GameManager.instance.playerUIGroup.alpha = 0;

        ColourData firstAgentColours = new ColourData();

        firstAgentColours.primaryColour = agents[0].agentData.primaryColour;
        firstAgentColours.secondaryColour = agents[0].agentData.secondaryColour;
        firstAgentColours.pantsColour = agents[0].agentData.pantsColour;
        firstAgentColours.vestColour = agents[0].agentData.vestColour;
        firstAgentColours.tieColour = agents[0].agentData.tieColour;

        firstOutroAgentDisplay.SetColour(firstAgentColours);
        firstOutroAgentDisplay.SetHat(agents[0].agentData.hatIndex);
        firstOutroAgentDisplay.ToggleSuit(agents[0].agentData.suitIndex);
        firstOutroAgentDisplay.ToggleVest(agents[0].agentData.vestIndex);

        secondOutroAgentDisplay.gameObject.SetActive(false);
        thirdOutroAgentDisplay.gameObject.SetActive(false);
        fourthOutroAgentDisplay.gameObject.SetActive(false);
        fifthOutroAgentDisplay.gameObject.SetActive(false);

        firstOutroAgentName.text = agents[0].playerName;

        if (agents.Count >= 2)
        {
            secondOutroAgentDisplay.gameObject.SetActive(true);

            ColourData secondAgentColours = new ColourData();

            secondAgentColours.primaryColour = agents[1].agentData.primaryColour;
            secondAgentColours.secondaryColour = agents[1].agentData.secondaryColour;
            secondAgentColours.pantsColour = agents[1].agentData.pantsColour;
            secondAgentColours.vestColour = agents[1].agentData.vestColour;
            secondAgentColours.tieColour = agents[1].agentData.tieColour;

            secondOutroAgentDisplay.SetColour(secondAgentColours);
            secondOutroAgentDisplay.SetHat(agents[1].agentData.hatIndex);
            secondOutroAgentDisplay.ToggleSuit(agents[1].agentData.suitIndex);
            secondOutroAgentDisplay.ToggleVest(agents[1].agentData.vestIndex);

            secondOutroAgentName.text = agents[1].playerName;

        }

        if (agents.Count >= 3)
        {
            thirdOutroAgentDisplay.gameObject.SetActive(true);

            ColourData thirdAgentColours = new ColourData();

            thirdAgentColours.primaryColour = agents[2].agentData.primaryColour;
            thirdAgentColours.secondaryColour = agents[2].agentData.secondaryColour;
            thirdAgentColours.pantsColour = agents[2].agentData.pantsColour;
            thirdAgentColours.vestColour = agents[2].agentData.vestColour;
            thirdAgentColours.tieColour = agents[2].agentData.tieColour;

            thirdOutroAgentDisplay.SetColour(thirdAgentColours);
            thirdOutroAgentDisplay.SetHat(agents[2].agentData.hatIndex);
            thirdOutroAgentDisplay.ToggleSuit(agents[2].agentData.suitIndex);
            thirdOutroAgentDisplay.ToggleVest(agents[2].agentData.vestIndex);

            thirdOutroAgentName.text = agents[2].playerName;
        }

        if (agents.Count >= 4)
        {
            fourthOutroAgentDisplay.gameObject.SetActive(true);

            ColourData fourthAgentColours = new ColourData();

            fourthAgentColours.primaryColour = agents[3].agentData.primaryColour;
            fourthAgentColours.secondaryColour = agents[3].agentData.secondaryColour;
            fourthAgentColours.pantsColour = agents[3].agentData.pantsColour;
            fourthAgentColours.vestColour = agents[3].agentData.vestColour;
            fourthAgentColours.tieColour = agents[3].agentData.tieColour;

            fourthOutroAgentDisplay.SetColour(fourthAgentColours);
            fourthOutroAgentDisplay.SetHat(agents[3].agentData.hatIndex);
            fourthOutroAgentDisplay.ToggleSuit(agents[3].agentData.suitIndex);
            fourthOutroAgentDisplay.ToggleVest(agents[3].agentData.vestIndex);

            fourthOutroAgentName.text = agents[3].playerName;

        }

        if (agents.Count == 5)
        {
            fifthOutroAgentDisplay.gameObject.SetActive(true);

            ColourData fifthAgentColours = new ColourData();

            fifthAgentColours.primaryColour = agents[4].agentData.primaryColour;
            fifthAgentColours.secondaryColour = agents[4].agentData.secondaryColour;
            fifthAgentColours.pantsColour = agents[4].agentData.pantsColour;
            fifthAgentColours.vestColour = agents[4].agentData.vestColour;
            fifthAgentColours.tieColour = agents[4].agentData.tieColour;

            fifthOutroAgentDisplay.SetColour(fifthAgentColours);
            fifthOutroAgentDisplay.SetHat(agents[4].agentData.hatIndex);
            fifthOutroAgentDisplay.ToggleSuit(agents[4].agentData.suitIndex);
            fifthOutroAgentDisplay.ToggleVest(agents[4].agentData.vestIndex);

            fifthOutroAgentName.text = agents[4].playerName;
        }

        probes.DisableProbes();

        H_TransitionManager.instance.SetClear();
        H_TransitionManager.instance.FadeIn(1);

        yield return new WaitForSeconds(1.5f);

        spiesEliminatedTimeline.Play();
        H_TransitionManager.instance.SetClear();

        yield return new WaitForSeconds((float)spiesEliminatedTimeline.playableAsset.duration);

        firstOutroAgentDisplay.ClearHat();
        secondOutroAgentDisplay.ClearHat();
        thirdOutroAgentDisplay.ClearHat();
        fourthOutroAgentDisplay.ClearHat();
        fifthOutroAgentDisplay.ClearHat();

        firstOutroAgentName.text = "";
        secondOutroAgentName.text = "";
        thirdOutroAgentName.text = "";
        fourthOutroAgentName.text = "";
        fifthOutroAgentName.text = "";

        H_TransitionManager.instance.SetBlack();
        H_TransitionManager.instance.FadeOut(0.5f);
        H_GameManager.instance.playerUIGroup.alpha = 1;

        if (isServer)
        {
            foreach (var player in H_GameManager.instance.roundPlayers)
            {
                player.isHudHidden = false;
            }

            H_GameManager.instance.globalHideHud = false;

            H_GameManager.instance.RoundEnd();
        }

    }

    IEnumerator PlayEvidenceGatheredCutscene(IntroCosmeticData agent)
    {
        H_GameManager.instance.playerUIGroup.alpha = 0;

        ColourData agentColours = new ColourData();

        agentColours.primaryColour = agent.agentData.primaryColour;
        agentColours.secondaryColour = agent.agentData.secondaryColour;
        agentColours.pantsColour = agent.agentData.pantsColour;
        agentColours.vestColour = agent.agentData.vestColour;
        agentColours.tieColour = agent.agentData.tieColour;

        evidenceAgentDisplay.SetColour(agentColours);
        evidenceAgentDisplay.SetHat(agent.agentData.hatIndex);
        evidenceAgentDisplay.ToggleSuit(agent.agentData.suitIndex);
        evidenceAgentDisplay.ToggleVest(agent.agentData.vestIndex);

        probes.DisableProbes();

        H_TransitionManager.instance.SetClear();
        H_TransitionManager.instance.FadeIn(1);

        yield return new WaitForSeconds(1.5f);

        evidenceGatheredTimeline.Play();
        H_TransitionManager.instance.SetClear();

        yield return new WaitForSeconds((float)evidenceGatheredTimeline.playableAsset.duration);

        evidenceAgentDisplay.ClearHat();

        H_TransitionManager.instance.SetBlack();
        H_TransitionManager.instance.FadeOut(0.5f);
        H_GameManager.instance.playerUIGroup.alpha = 1;

        if (isServer)
        {
            foreach (var player in H_GameManager.instance.roundPlayers)
            {
                player.isHudHidden = false;
            }

            H_GameManager.instance.globalHideHud = false;

            H_GameManager.instance.RoundEnd();
        }

    }

    IEnumerator PlayTimeOutCutscene()
    {
        H_GameManager.instance.playerUIGroup.alpha = 0;

        probes.DisableProbes();

        H_TransitionManager.instance.SetClear();
        H_TransitionManager.instance.FadeIn(1);

        yield return new WaitForSeconds(1.5f);

        timeOutTimeline.Play();
        H_TransitionManager.instance.SetClear();

        yield return new WaitForSeconds((float)timeOutTimeline.playableAsset.duration);

        H_TransitionManager.instance.SetBlack();
        H_TransitionManager.instance.FadeOut(0.5f);
        H_GameManager.instance.playerUIGroup.alpha = 1;

        if (isServer)
        {
            foreach (var player in H_GameManager.instance.roundPlayers)
            {
                player.isHudHidden = false;
            }

            H_GameManager.instance.globalHideHud = false;

            H_GameManager.instance.RoundEnd();
        }

    }
}

[System.Serializable]
public struct IntroCosmeticData
{
    public string playerName;
    public AgentData agentData;
}