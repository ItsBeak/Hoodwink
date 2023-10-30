using UnityEngine;
using Mirror;

public interface H_IInteractable
{
    public string InteractableName { get; }
    public string InteractableVerb { get; }

    public void OnInteract(NetworkIdentity client);
}

/*
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

    public void OnInteract(NetworkIdentity client)
    {

    }
*/