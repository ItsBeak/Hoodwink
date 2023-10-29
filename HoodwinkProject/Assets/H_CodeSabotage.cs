using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_CodeSabotage : MonoBehaviour, H_IInteractable
{
    public string itemName;
    public string itemVerb;
    public bool itemEnabled;

    public string InteractableName
    {
        get { return itemName; }
    }

    public string InteractableVerb
    {
        get { return itemVerb; }
    }

    public bool InteractableEnabled
    {
        get { return itemEnabled; }
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
