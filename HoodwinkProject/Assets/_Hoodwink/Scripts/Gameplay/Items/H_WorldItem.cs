using UnityEngine;
using Mirror;
public class H_WorldItem : NetworkBehaviour
{
    public string itemName;

    public GameObject clientItem;
    public GameObject observerItem;
    public void PickUpItem(NetworkIdentity newOwner)
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
