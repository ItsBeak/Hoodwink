using UnityEngine;
using Mirror;
using TMPro;

public class H_CodeButton : MonoBehaviour, H_IInteractable
{
    public string InteractableName => throw new System.NotImplementedException();
    public string InteractableVerb => throw new System.NotImplementedException();

    public void OnInteract(NetworkIdentity client)
    {

    }
}
