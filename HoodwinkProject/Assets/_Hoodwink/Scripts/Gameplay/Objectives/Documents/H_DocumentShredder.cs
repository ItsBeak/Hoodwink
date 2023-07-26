using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class H_DocumentShredder : NetworkBehaviour
{

    [SyncVar] public uint inUseBy = 0;

    [Header("Audio")]
    public AudioClip useClip;
    public AudioClip stopUseClip;

    AudioSource source;

    [Header("Debugging")]
    public bool enableDebugLogs;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    [Command(requiresAuthority = false)]
    public void CmdShredDocument()
    {
        H_GameManager.instance.CmdUpdateEvidence(10);
        inUseBy = 0;

        if (enableDebugLogs)
            Debug.Log("A document has been shredded");
    }

    [Command(requiresAuthority = false)]
    public void CmdStartUse(uint playerID)
    {
        inUseBy = playerID;

        RpcStartUse();

        if (enableDebugLogs)
            Debug.Log("The shredder is being used by: " + playerID);
    }

    [Command(requiresAuthority = false)]
    public void CmdStopUse()
    {
        inUseBy = 0;

        RpcStopUse();

        if (enableDebugLogs)
            Debug.Log("The shredder is not being used");
    }

    [ClientRpc]
    public void RpcStartUse()
    {
        source.PlayOneShot(useClip);
    }

    [ClientRpc]
    public void RpcStopUse()
    {
        source.Stop();
        source.PlayOneShot(stopUseClip);
    }
}
