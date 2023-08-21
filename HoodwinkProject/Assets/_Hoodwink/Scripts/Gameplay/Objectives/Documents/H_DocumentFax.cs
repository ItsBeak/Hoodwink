using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class H_DocumentFax : NetworkBehaviour
{
    [SyncVar] public uint inUseBy = 0;
    public int scoreChange;

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
    public void CmdFaxdDocument()
    {
        H_GameManager.instance.CmdUpdateEvidence(scoreChange);
        inUseBy = 0;

        if (enableDebugLogs)
            Debug.Log("A document has been faxed");
    }

    [Command(requiresAuthority = false)]
    public void CmdStartUse(uint playerID)
    {
        inUseBy = playerID;

        RpcStartUse();

        if (enableDebugLogs)
            Debug.Log("The fax machine is being used by: " + playerID);
    }

    [Command(requiresAuthority = false)]
    public void CmdStopUse()
    {
        inUseBy = 0;

        RpcStopUse();

        if (enableDebugLogs)
            Debug.Log("The fax machine is being used");
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
