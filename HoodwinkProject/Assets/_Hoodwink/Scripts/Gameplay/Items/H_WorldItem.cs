using UnityEngine;
using Mirror;
public class H_WorldItem : NetworkBehaviour, H_IInteractable
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

    public GameObject clientItem;
    public GameObject observerItem;

    public void OnInteract(NetworkIdentity client)
    {
        CmdPickUpItem(client);
    }

    [Command(requiresAuthority = false)]
    public void CmdPickUpItem(NetworkIdentity newOwner)
    {
        H_PlayerEquipment player = newOwner.GetComponent<H_PlayerEquipment>();

        if (player != null)
        {
            GameObject newClientItem = Instantiate(clientItem);
            GameObject newObserverItem = Instantiate(observerItem);

            NetworkServer.Spawn(newClientItem, newOwner.connectionToClient);
            NetworkServer.Spawn(newObserverItem, newOwner.connectionToClient);

            player.RpcEquipPrimary(newClientItem, newObserverItem);

        }

        NetworkServer.Destroy(this.gameObject);
    }
}
