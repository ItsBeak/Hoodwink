using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class H_CodeComputer : NetworkBehaviour
{
    string key = "000000";
    [SyncVar] int currentSequenceIndex = 0;
    [SyncVar(hook = nameof(OnFailedAttemptsChanged))] int failedAttempts = 0;
    [SyncVar(hook = nameof(OnEnteredKeyChanged))] string enteredKey = "";
    [SyncVar] bool completed = false;
    public TextMeshPro sequenceText;

    AudioSource source;
    public AudioClip successClip, failClip, resetClip, buttonClip;

    public GameObject failLight01, failLight02, failLight03, successLight;

    private void Start()
    {
        source = GetComponent<AudioSource>();    
    }

    [Command(requiresAuthority = false)]
    public void CmdButtonPressed(int digit)
    {
        RpcPressButton();

        if (completed)
        {
            return;
        }

        Debug.Log("Comparing key " + digit + " to correct sequence " + int.Parse(key[currentSequenceIndex].ToString()));

        if (digit == int.Parse(key[currentSequenceIndex].ToString()))
        {
            enteredKey += digit.ToString();

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
        Debug.Log("Completed sequence");
        completed = true;
        RpcComplete();
    }

    void Fail()
    {
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
