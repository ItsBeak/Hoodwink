using System;
using System.Linq;
using System.Collections;

using UnityEngine;

using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Random = UnityEngine.Random;

using Mirror;
using Utp;

public class H_NetworkManager : NetworkManager
{
    [Range(1, 5)]
    public int minimumPlayersToStart;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;
    //public static event Action OnServerStopped;

    [HideInInspector] public bool isLoggedIn = false;
    [HideInInspector] public bool isLoggingIn = false;

    public override void Start()
    {
        spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        OnClientDisconnected?.Invoke();
    }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        conn.identity.GetComponent<H_PlayerBrain>().UnregisterPlayer();

        base.OnServerDisconnect(conn);
    }
}
