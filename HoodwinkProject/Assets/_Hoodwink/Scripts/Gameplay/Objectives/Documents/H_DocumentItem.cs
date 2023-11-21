using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System.Reflection;
using UnityEngine.ProBuilder.MeshOperations;

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

    public override void Update()
    {
        if (!isOwned || !equipment) return;

        base.Update();

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (focusedFax)
            {
                if (!focusedFax.containsDocument && focusedFax.documentsSent < 4 && !focusedFax.isOnCooldown)
                {
                    focusedFax.CmdAddDocument();
                    CmdDestroyDocuments(equipment.primaryClientObject, equipment.primaryObserverObject);
                    equipment.ClearCurrentObject();
                }
            }
            else if (focusedShredder)
            {
                if (!focusedShredder.containsDocument)
                {
                    focusedShredder.CmdAddDocument();
                    CmdDestroyDocuments(equipment.primaryClientObject, equipment.primaryObserverObject);
                    equipment.ClearCurrentObject();
                }
            }
        }

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
                if (focusedShredder.containsDocument)
                {
                    focusReadout.text = "Shredder already contains document";
                }
                else
                {
                    focusReadout.text = "Press F to shred documents";
                }
            }
        }
        else if (focusedFax)
        {
            
            if (focusedFax.containsDocument || focusedFax.isOnCooldown)
            {
                focusReadout.text = "Fax machine already contains document";
            }
            else if (focusedFax.documentsSent == 4)
            {
                focusReadout.text = "Fax machine cannot send more documents";
            }
            else
            {
                focusReadout.text = "Press F to insert documents";
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
