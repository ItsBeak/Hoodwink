using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using OdinNative.Odin;
using OdinNative.Unity.Audio;

public class H_PlayerVoice : NetworkBehaviour
{
    public string roomName = "hoodwink";

    public KeyCode PushToTalkHotkey;
    public bool UsePushToTalk = true;
    public MicrophoneReader AudioSender;

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
        roomName = NetManager.relayJoinCode.ToUpper();

        OdinHandler.Instance.JoinRoom(roomName);

        Debug.Log("Joined voice room: " + OdinHandler.Instance.Rooms);

        if (AudioSender == null)
            AudioSender = FindObjectOfType<MicrophoneReader>();
    }

    public override void OnStopLocalPlayer()
    {
        OdinHandler.Instance.LeaveRoom(roomName);
    }

    private void OnApplicationQuit()
    {
        OdinHandler.Instance.LeaveRoom(roomName);
    }

    void Update()
    {
        if (AudioSender)
            AudioSender.SilenceCapturedAudio = !(UsePushToTalk ? Input.GetKey(PushToTalkHotkey) : true);
    }
}
