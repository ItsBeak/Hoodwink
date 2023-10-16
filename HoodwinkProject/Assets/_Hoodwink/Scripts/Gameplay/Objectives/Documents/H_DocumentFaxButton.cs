using UnityEngine;
using Mirror;

public class H_DocumentFaxButton : MonoBehaviour, H_IInteractable
{
    public string itemName;
    public string itemVerb;

    bool isButtonActive;
    Renderer rend;
    public Material activeMaterial, inactiveMaterial;

    public string InteractableName
    {
        get { return itemName; }
    }

    public string InteractableVerb
    {
        get { return itemVerb; }
    }

    H_DocumentFax faxMachine;

    private void Start()
    {
        faxMachine = GetComponentInParent<H_DocumentFax>();
        rend = GetComponent<Renderer>();
    }

    public void OnInteract(NetworkIdentity client)
    {
        faxMachine.CmdButtonPressed(isButtonActive);
    }

    public void Activate()
    {
        isButtonActive = true;
        rend.material = activeMaterial;
    }

    public void Deactivate()
    {
        isButtonActive = false;
        rend.material = inactiveMaterial;
    }
}
