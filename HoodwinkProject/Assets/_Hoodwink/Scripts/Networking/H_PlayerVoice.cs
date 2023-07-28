using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using OdinNative.Unity.Samples;
using OdinNative.Odin;

public class H_PlayerVoice : NetworkBehaviour
{
    [SyncVar]
    public string playerName;

    [SyncVar]
    public string odinSeed;

    private H_NetworkManager netManager;

    private H_NetworkManager NetManager
    {
        get
        {
            if (netManager != null) { return netManager; }
            return netManager = NetworkManager.singleton as H_NetworkManager;
        }
    }

    public override void OnStartLocalPlayer()
    {

        string pName = "Player" + Random.Range(100, 999);

        CustomUserDataJsonFormat userData = new CustomUserDataJsonFormat(pName, "online");
        userData.seed = netId.ToString();

        OdinHandler.Instance.JoinRoom(NetManager.relayJoinCode, userData.ToUserData());

        CmdSetupPlayer(pName, netId.ToString());

    }

    [Command]
    public void CmdSetupPlayer(string pName, string seed)
    {
        playerName = pName;
        odinSeed = seed;
    }
}
