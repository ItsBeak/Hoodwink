using UnityEngine;
using Mirror;
using OdinNative;
using OdinNative.Odin;

using System.Text;
using System;

public class H_PlayerVoice : NetworkBehaviour
{

    public override void OnStartLocalPlayer()
    {
        HoodwinkUserData userData = new HoodwinkUserData("Hoodwinker-" + netId.ToString());
        userData.seed = netId;

        OdinHandler.Instance.JoinRoom("HoodwinkTest", userData.ToUserData());
    }
}

public class HoodwinkUserData : IUserData
{

    public string playerID;
    public uint seed;

    public HoodwinkUserData() : this("Hoodwinker") { }
    public HoodwinkUserData(string id)
    {
        this.playerID = id;
    }

    public UserData ToUserData()
    {
        return new UserData(this.ToJson());
    }

    public static HoodwinkUserData FromUserData(UserData userData)
    {
        return JsonUtility.FromJson<HoodwinkUserData>(userData.ToString());
    }

    public static bool FromUserData(UserData userData, out HoodwinkUserData customUserData)
    {
        try
        {
            customUserData = JsonUtility.FromJson<HoodwinkUserData>(userData.ToString());
            return true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            customUserData = null;
            return false;
        }
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public override string ToString()
    {
        return this.ToJson();
    }

    public byte[] ToBytes()
    {
        return Encoding.UTF8.GetBytes(this.ToString());
    }

    public bool IsEmpty()
    {
        throw new System.NotImplementedException();
    }
}