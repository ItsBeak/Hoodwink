using UnityEngine;
using Mirror;

public class H_VoiceChat : NetworkBehaviour
{

    [SerializeField] public string roomName;

    public override void OnStartLocalPlayer()
    {
        OdinHandler.Instance.JoinRoom(roomName);
    }

    private void OnDestroy()
    {
        if (isLocalPlayer)
        {
            OdinHandler.Instance.LeaveRoom(roomName);
        }
    }
}


