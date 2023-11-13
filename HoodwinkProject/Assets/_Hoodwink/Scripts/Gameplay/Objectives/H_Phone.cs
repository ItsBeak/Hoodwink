using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_Phone : NetworkBehaviour, H_IInteractable
{

    public string itemName;
    public bool itemEnabled;

    public string InteractableName
    {
        get { return itemName; }
    }

    public string InteractableVerb
    {
        get 
        { 
            if (isRinging)
            {
                return "pick up";
            }
            else if (isOnCall)
            {
                return "hang up";
            }
            else
            {
                return "";
            }
        }
    }

    public bool InteractableEnabled
    {
        get { return itemEnabled; }
    }

    [Header("Score Settings")]
    public int scoreChange;

    [Header("Phone Settings")]
    public float callLength;

    [Header("Audio Settings")]
    public AudioSource ringingSource;
    public AudioSource callSource;
    public AudioSource phoneSource;
    public AudioClip pickupClip, hangUpClip;

    [Header("SyncVars")]
    [SyncVar(hook = nameof(OnActiveChanged))] public bool isActive;
    [SyncVar(hook = nameof(OnRingingChanged))] public bool isRinging;
    [SyncVar(hook = nameof(OnCallChanged))] public bool isOnCall;

    public void OnInteract(NetworkIdentity client)
    {
        if (isRinging)
        {
            CmdPickUp();
        }
        else
        {
            CmdHangUp();
        }
    }

    void OnActiveChanged(bool oldState, bool newState)
    {
        itemEnabled = newState;
    }

    void OnRingingChanged(bool oldState, bool newState)
    {
        if (newState)
        {
            ringingSource.Play();
        }
        else
        {
            ringingSource.Stop();
        }
    }

    void OnCallChanged(bool oldState, bool newState)
    {
        if (newState)
        {
            phoneSource.PlayOneShot(pickupClip);
            callSource.Play();
        }
        else
        {
            callSource.Stop();
            phoneSource.PlayOneShot(hangUpClip);
        }
    }

    [Server]
    public void Ring()
    {
        if (!isActive && !isRinging)
        {
            isActive = true;
            isRinging = true;
        }
    }

    [Command(requiresAuthority = false)]
    void CmdPickUp()
    {
        isRinging = false;
        isOnCall = true;

        Invoke(nameof(CheckCall), callLength);

    }

    void CheckCall()
    {
        if (isOnCall)
        {
            H_GameManager.instance.CmdUpdateEvidence(scoreChange);
            isOnCall = false;
            isActive = false;
        }
    }

    [Command(requiresAuthority = false)]
    void CmdHangUp()
    {
        isOnCall = false;
        isActive = false;
    }
}
