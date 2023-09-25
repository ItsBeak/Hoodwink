using UnityEngine;

using OdinNative.Odin.Peer;
using OdinNative.Odin.Room;
using OdinNative.Unity.Audio;

using Mirror;
using System.Text;
using OdinNative.Odin;

public class H_VoiceChat : NetworkBehaviour
{
    [SerializeField] public string roomName;

    [SerializeField] private PlaybackComponent playbackPrefab;
    private PlaybackComponent spawnedPlayback;

    public Vector3 voiceTransmitLocation;

    private void Start()
    {
        OdinHandler.Instance.OnMediaAdded.AddListener(MediaAdded);

        if (isLocalPlayer)
        {
            CustomUserData roomData = new CustomUserData
            {
                NetworkId = netId
            };

            OdinHandler.Instance.JoinRoom(roomName, roomData);
        }
    }

    private void MediaAdded(object roomObject, MediaAddedEventArgs eventArgs)
    {
        ulong peerId = eventArgs.PeerId;
        long mediaId = eventArgs.Media.Id;

        if (roomObject is Room room)
        {
            Peer peer = room.RemotePeers[peerId];
            CustomUserData userData = JsonUtility.FromJson<CustomUserData>(peer.UserData.ToString());

            if (!isLocalPlayer && userData.NetworkId == netId)
            {
                spawnedPlayback = Instantiate(playbackPrefab, transform);
                spawnedPlayback.transform.localPosition = voiceTransmitLocation;
                spawnedPlayback.RoomName = room.Config.Name;
                spawnedPlayback.PeerId = peerId;
                spawnedPlayback.MediaStreamId = mediaId;
            }

        }
    }

    private void OnDisable()
    {
        if (null != spawnedPlayback)
            Destroy(spawnedPlayback.gameObject);
    }

    private void OnDestroy()
    {
        if (isLocalPlayer)
        {
            OdinHandler.Instance.LeaveRoom(roomName);
        }
    }
}

public class CustomUserData : IUserData
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

