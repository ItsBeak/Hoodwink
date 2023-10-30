using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class H_RoundEndManager : NetworkBehaviour
{

    public AgentCard[] agentCards;

    public PlayableDirector director;

    public CanvasGroup mainUI;
    public CanvasGroup roundEndUI;

    public TextMeshProUGUI popup;

    [ClientRpc]
    public void RpcSetAgentCards(RoundEndData[] agents, string popupText)
    {
        roundEndUI.alpha = 1;

        foreach (var card in agentCards)
        {
            card.canvasGroup.alpha = 0;
            card.deceasedStamp.alpha = 0;
        }

        for (int i = 0; i < agents.Length; i++)
        {
            agentCards[i].canvasGroup.alpha = 1;
            agentCards[i].deceasedStamp.alpha = agents[i].isDead ? 1 : 0;
            agentCards[i].agentName.text = agents[i].agentData.agentName;
            agentCards[i].agentColour.color = agents[i].agentData.primaryColour;
        }

        director.time = 0;
        director.Play();

        popup.text = popupText;

    }

    [ClientRpc]
    public void RpcResetUI()
    {
        director.Stop();
        director.time = 0;

        mainUI.alpha = 1;
        roundEndUI.alpha = 0;

        popup.text = "";

    }

}

[System.Serializable]
public class AgentCard
{
    public CanvasGroup canvasGroup;
    public CanvasGroup deceasedStamp;
    public TextMeshProUGUI agentName;
    public Image agentColour;
}

[System.Serializable]
public struct RoundEndData
{
    public AgentData agentData;
    public bool isDead;
}