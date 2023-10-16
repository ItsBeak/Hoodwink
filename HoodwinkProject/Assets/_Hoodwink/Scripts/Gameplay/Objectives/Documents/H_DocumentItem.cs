using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System.Reflection;

public class H_DocumentItem : H_ItemBase
{

    H_PlayerBrain brain;
    bool ownerIsSpy;

    H_DocumentShredder focusedShredder;
    H_DocumentFax focusedFax;

    [Space(10)]
    [Header("Document Components")]
    public TextMeshProUGUI focusReadout;

    public override void Initialize()
    {
        base.Initialize();

        brain = GetComponentInParent<H_PlayerBrain>();
        ownerIsSpy = brain.currentAlignment == AgentAlignment.Spy;
    }

    public override void PrimaryUse()
    {
        if (focusedFax)
        {
            if (!focusedFax.containsDocument)
            {
                focusedFax.CmdAddDocument();
                CmdDestroyDocuments(equipment.primaryClientObject, equipment.primaryObserverObject);
                equipment.ClearCurrentObject();
            }
        }
    }

    public override void Update()
    {
        if (!isOwned || !equipment) return;

        base.Update();

        UpdateUI();
    }

    private void FixedUpdate()
    {
        if (!isOwned || !equipment) return;

        RaycastHit hit;

        if (Physics.Raycast(equipment.playerCamera.transform.position, equipment.playerCamera.transform.forward, out hit, equipment.interactionRange, equipment.interactableLayers))
        {
            H_DocumentShredder shredder = hit.collider.GetComponent<H_DocumentShredder>();
            H_DocumentFax fax = hit.collider.GetComponent<H_DocumentFax>();

            if (shredder != null)
            {
                focusedShredder = shredder;
            }
            else
            {
                focusedShredder = null;
            }

            if (fax != null)
            {
                focusedFax = fax;
            }
            else
            {
                focusedFax = null;
            }
        }
        else
        {
            focusedShredder = null;
            focusedFax = null;
        }

    }

    void UpdateUI()
    {
        if (focusedShredder)
        {
            if (ownerIsSpy)
            {
                if (waitForPrimaryKeyReleased)
                {
                    focusReadout.text = "Shredding";
                }
                else
                {
                    focusReadout.text = "Hold " + equipment.primaryUseKey + " to shred documents";
                }
            }
        }
        else if (focusedFax)
        {
            
            if (focusedFax.containsDocument)
            {
                focusReadout.text = "Fax machine already contains document";
            }
            else
            {
                focusReadout.text = "Press " + equipment.primaryUseKey + " to insert documents";
            }
            
        }
        else
        {
            focusReadout.text = "";
        }
    }

    [Command(requiresAuthority = false)]
    void CmdDestroyDocuments(GameObject client, GameObject observer)
    {
        NetworkServer.Destroy(client);
        NetworkServer.Destroy(observer);
    }
}
