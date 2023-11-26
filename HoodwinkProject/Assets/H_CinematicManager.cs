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
                H_GameManager.instance.globalHideHud = false;
                p.RpcSetCanMove(true);
                p.equipment.SetBusy(false);
            }
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

        if (players.Count == 1)
        {
            spyPlayerRole.text = "You are the " + H_GameManager.ColorWord("Spy", Color.red);

            firstSpyTeammateDisplay.gameObject.SetActive(false);
            secondSpyTeammateDisplay.gameObject.SetActive(false);

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
                firstSpyPlayerAgentName.text = "Agent " + H_GameManager.ColorWord(players[0].agentData.agentName, players[1].agentData.primaryColour);
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
                secondSpyPlayerAgentName.text = "Agent " + H_GameManager.ColorWord(players[0].agentData.agentName, players[2].agentData.primaryColour);
            }
        }

        probes.DisableProbes();

        spyIntroTimeline.Play();
        H_TransitionManager.instance.SetClear();

        yield return new WaitForSeconds((float)spyIntroTimeline.playableAsset.duration);

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
                H_GameManager.instance.globalHideHud = false;
                p.RpcSetCanMove(true);
                p.equipment.SetBusy(false);
            }
        }

    }

    IEnumerator PlayAgentsEliminatedCutscene(List<IntroCosmeticData> spies)
    {
        Debug.Log(spies.Count);

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

        H_TransitionManager.instance.SetBlack();
        H_TransitionManager.instance.FadeOut(0.5f);
        H_GameManager.instance.playerUIGroup.alpha = 1;

        if (isServer)
        {
            foreach (var player in H_GameManager.instance.roundPlayers)
            {
                player.isHudHidden = false;
                H_GameManager.instance.globalHideHud = false;

            }

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