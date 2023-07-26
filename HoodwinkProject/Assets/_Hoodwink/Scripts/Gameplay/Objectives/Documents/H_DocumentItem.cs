using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mirror;

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
            focusReadout.text = "Press " + equipment.primaryUseKey + " to shred documents";
        }
        else if (focusedFax)
        {
            if (ownerIsSpy)
            {
                focusReadout.text = "Press " + equipment.primaryUseKey + " to fax documents";
            }
        }
        else
        {
            focusReadout.text = "";
        }
    }

    public override void PrimaryUse()
    {
        if (focusedShredder)
        {
            focusedShredder.CmdShredDocument();
            equipment.CmdDropItem();
        }
        else if (focusedFax)
        {
            if (ownerIsSpy)
            {
                focusedFax.CmdFaxdDocument();
                equipment.CmdDropItem();
            }
        }
    }
}
