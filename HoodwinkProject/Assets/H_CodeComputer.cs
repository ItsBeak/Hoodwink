using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class H_CodeComputer : NetworkBehaviour
{
    string key = "000000";
    [SyncVar] int currentSequenceIndex = 0;
    [SyncVar(hook = nameof(OnEnteredKeyChanged))] string enteredKey = "";
    [SyncVar] bool completed = false;
    public TextMeshPro sequenceText;

    [Command(requiresAuthority = false)]
    public void CmdButtonPressed(int digit)
    {
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
        //string displayText = "";
        //
        //for (int i = 0; i < key.Length; i++)
        //{
        //    if (enteredKey[i] != 0)
        //    {
        //        displayText += enteredKey[i].ToString();
        //    }
        //    else
        //    {
        //        displayText += "_";
        //    }
        //}
        //
        //sequenceText.text = displayText;

        sequenceText.text = newKey;

    }

    void Complete()
    {
        Debug.Log("Completed sequence");
        completed = true;
    }

    void Fail()
    {
        Debug.Log("Failed sequence");

        currentSequenceIndex = 0;
        enteredKey = "";

    }

}
