using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using OdinNative.Odin;
using OdinNative.Unity.Audio;
using System.Text;

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
        VoiceUserData roomData = new VoiceUserData()
        {
            NetworkId = netId
        };

        roomName = NetManager.relayJoinCode.ToUpper();

        OdinHandler.Instance.JoinRoom(roomName, roomData);

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

public class VoiceUserData : IUserData
{
    public uint NetworkId;

    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }

    public bool IsEmpty()
    {
        return string.IsNullOrEmpty(this.ToString());
    }

    public byte[] ToBytes()
    {
        return Encoding.UTF8.GetBytes(ToString());
    }
}
