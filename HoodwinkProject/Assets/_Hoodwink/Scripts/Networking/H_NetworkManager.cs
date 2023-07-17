using System;
using System.Linq;
using System.Collections;

using UnityEngine;

using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;

using Mirror;
using Utp;

public class H_NetworkManager : RelayNetworkManager
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

        if (UnityServices.State == ServicesInitializationState.Initialized)
        {
            if (AuthenticationService.Instance.IsSignedIn)
            {
                isLoggedIn = true;
            }
        }
    }

    public async void UnityLogin()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Logged into Unity, player ID: " + AuthenticationService.Instance.PlayerId);
            isLoggedIn = true;
        }
        catch (Exception e)
        {

            if (AuthenticationService.Instance.IsSignedIn)
            {
                isLoggedIn = true;
                return;
            }

            isLoggedIn = false;
            Debug.Log(e);
        }
    }

    private IEnumerator ListRegions()
    {
        var regionsRequest = Relay.Instance.ListRegionsAsync();

        while (!regionsRequest.IsCompleted)
        {
            yield return null;
        }

        if (regionsRequest.IsFaulted)
        {
            Debug.LogError("Regions request failed");
            yield break;
        }

        var regionList = regionsRequest.Result;

        foreach (var region in regionList)
        {
            Debug.Log(region.Id);
        }
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
        //conn.identity.GetComponent<H_PlayerEquipment>().TryDropItem();

        base.OnServerDisconnect(conn);
    }

    public override void Update()
    {
        base.Update();

        isLoggingIn = UnityServices.State == ServicesInitializationState.Initializing;

        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

    }
}
