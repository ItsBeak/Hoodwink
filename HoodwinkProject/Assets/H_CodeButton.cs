using UnityEngine;
using Mirror;
using TMPro;

public class H_CodeButton : MonoBehaviour, H_IInteractable
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

    public int digit;
    H_CodeComputer computer;

    private void Start()
    {
        computer = GetComponentInParent<H_CodeComputer>();
    }

    public void OnInteract(NetworkIdentity client)
    {
        computer.CmdButtonPressed(digit);
    }
}
