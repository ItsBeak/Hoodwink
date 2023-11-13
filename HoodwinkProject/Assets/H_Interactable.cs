using UnityEngine;
using Mirror;

public interface H_IInteractable
{
    public string InteractableName { get; }
    public string InteractableVerb { get; }
    public bool InteractableEnabled { get; }

    public void OnInteract(NetworkIdentity client);
}

/*
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

    public void OnInteract(NetworkIdentity client)
    {

    }
*/