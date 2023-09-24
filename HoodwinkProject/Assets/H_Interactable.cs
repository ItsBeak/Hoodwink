using UnityEngine;
using Mirror;

public interface H_IInteractable
{
    public string InteractableName { get; }
    public string InteractableVerb { get; }

    public void OnInteract(NetworkIdentity client);
}