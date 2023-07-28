using UnityEngine;
using Mirror;
using OdinNative.Odin.Room;
using OdinNative.Unity.Audio;
using OdinNative.Odin;
using System.Text;
using OdinNative.Odin.Peer;

public class H_PlayerVoiceRemote : NetworkBehaviour
{
    public PlaybackComponent playbackPrefab;
    PlaybackComponent spawnedPlayback;

    public override void OnStartLocalPlayer()
    {
        OdinHandler.Instance.OnMediaAdded.AddListener(MediaAdded);
    }

    void MediaAdded(object roomObject, MediaAddedEventArgs eventArgs)
    {
        ulong peerId = eventArgs.PeerId;
        long mediaId = eventArgs.Media.Id;

        if (roomObject is Room room)
        {
            Peer peer = room.RemotePeers[peerId];
            VoiceUserData userData = JsonUtility.FromJson<VoiceUserData>(peer.UserData.ToString());

            if (!isLocalPlayer && userData.NetworkId == netId)
            {
                spawnedPlayback = Instantiate(playbackPrefab, transform);
                spawnedPlayback.transform.localPosition = Vector3.zero;
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
}


