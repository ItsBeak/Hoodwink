using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_CodeSabotage : MonoBehaviour, H_IInteractable
{
    public string itemName;
    public string itemVerb;

    public string InteractableName
    {
        get { return itemName; }
    }

    public string InteractableVerb
    {
        get { return itemVerb; }
    }

    H_CodeComputer computer;

    private void Start()
    {
        computer = GetComponentInParent<H_CodeComputer>();
    }

    public void OnInteract(NetworkIdentity client)
    {
        computer.CmdSabotage();
    }
}
