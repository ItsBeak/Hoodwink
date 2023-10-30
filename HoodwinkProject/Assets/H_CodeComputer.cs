using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class H_CodeComputer : NetworkBehaviour
{
    public int scoreChange;

    string key = "000000";
    [SyncVar] int currentSequenceIndex = 0;
    [SyncVar(hook = nameof(OnFailedAttemptsChanged))] int failedAttempts = 0;
    [SyncVar(hook = nameof(OnEnteredKeyChanged))] string enteredKey = "";
    [SyncVar] bool isCompleted = false;
    [SyncVar(hook = nameof(OnSabotaged))] bool isSabotaged = false;
    public TextMeshPro sequenceText;
    public GameObject sabotageEffects;

    AudioSource source;
    public AudioClip successClip, failClip, resetClip, buttonClip;

    public GameObject failLight01, failLight02, failLight03, successLight;

    public bool enableDebugLogs;

    private void Start()
    {
        source = GetComponent<AudioSource>();    
    }

    [Command(requiresAuthority = false)]
    public void CmdButtonPressed(int digit)
    {
        RpcPressButton();

        if (isCompleted || isSabotaged)
        {
            return;
        }

        if (enableDebugLogs)
            Debug.Log("Comparing key " + digit + " to correct sequence " + int.Parse(key[currentSequenceIndex].ToString()));

        if (digit == int.Parse(key[currentSequenceIndex].ToString()))
        {
            enteredKey += digit.ToString();

            if (enableDebugLogs)
                Debug.Log("Updating key to: " + enteredKey + " out of " + key);

            currentSequenceIndex++;

            if (currentSequenceIndex == key.Length)
            {
                Complete();
            }

        }
        else
        {
            Fail();
        }
    }

    public void OnKeyReceieved(string newKey)
    {
        key = newKey;
    }


    void OnEnteredKeyChanged(string oldKey, string newKey)
    {
        sequenceText.text = newKey;
    }

    void OnSabotaged(bool oldValue, bool newValue)
    {
        sabotageEffects.SetActive(newValue);
    }

    void OnFailedAttemptsChanged(int oldFails, int newFails)
    {
        if (newFails >= 3)
        {
            failLight01.SetActive(true);
            failLight02.SetActive(true);
            failLight03.SetActive(true);
        }
        else if (newFails == 2)
        {
            failLight01.SetActive(true);
            failLight02.SetActive(true);
            failLight03.SetActive(false);
        }
        else if (newFails == 1)
        {
            failLight01.SetActive(true);
            failLight02.SetActive(false);
            failLight03.SetActive(false);
        }
        else
        {
            failLight01.SetActive(false);
            failLight02.SetActive(false);
            failLight03.SetActive(false);
        }
    }

    void Complete()
    {
        if (enableDebugLogs)
            Debug.Log("Completed sequence");

        isCompleted = true;
        H_GameManager.instance.CmdUpdateEvidence(scoreChange);
        RpcComplete();
    }

    void Fail()
    {
        if (enableDebugLogs)
            Debug.Log("Failed sequence");

        currentSequenceIndex = 0;
        enteredKey = "";

        failedAttempts++;

        if (failedAttempts >= 4)
        {
            failedAttempts = 0;
            ResetKey();
        }
        else
        {
            RpcFail();
        }

    }

    [Command(requiresAuthority = false)]
    public void CmdSabotage()
    {
        if (!isSabotaged && !isCompleted)
        {
            isSabotaged = true;
            Invoke("DisableSabotage", 30f);
        }
    }

    void DisableSabotage()
    {
        isSabotaged = false;
    }

    void ResetKey()
    {
        H_CodeKey keyMachine = FindObjectOfType<H_CodeKey>();
        keyMachine.GenerateKey();

        RpcResetKey();
    }

    [ClientRpc]
    void RpcComplete()
    {
        source.PlayOneShot(successClip);

        successLight.SetActive(true);
        failLight01.SetActive(false);
        failLight02.SetActive(false);
        failLight03.SetActive(false);
    }

    [Client]
    void RpcFail()
    {
        source.PlayOneShot(failClip);
    }

    [Client]
    void RpcResetKey()
    {
        source.PlayOneShot(resetClip);
    }

    [ClientRpc]
    void RpcPressButton()
    {
        source.PlayOneShot(buttonClip);
    }

}
