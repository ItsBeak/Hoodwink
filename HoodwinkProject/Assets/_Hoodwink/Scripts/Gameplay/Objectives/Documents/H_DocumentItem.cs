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

    H_DocumentShredder lastShredder;
    H_DocumentFax lastFax;

    bool isUsing;
    bool isDone;

    [Space(10)]
    [Header("Document Components")]
    public TextMeshProUGUI focusReadout;
    public Image useTimerImage;
    public float useTime;
    float timer = 0;

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

        if (waitForPrimaryKeyReleased && !isDone)
        {
            if (focusedShredder && ownerIsSpy)
            {
            
                if (focusedShredder.inUseBy == 0)
                {
                    if (!isUsing)
                    {
                        isUsing = true;
                        lastShredder = focusedShredder;
                        focusedShredder.CmdStartUse(netIdentity.netId);
                    }
                }
                else if (focusedShredder.inUseBy == netIdentity.netId)
                {
                    timer += Time.deltaTime;

                    if (timer >= useTime)
                    {
                        isDone = true;
                        focusedShredder.CmdShredDocument();

                        CmdDestroyDocuments(equipment.primaryClientObject, equipment.primaryObserverObject);
                        equipment.ClearCurrentObject();

                        isUsing = false;
                        focusedShredder.CmdStopUse();
                    }

                    useTimerImage.fillAmount = timer / useTime;
                }
                else
                {
                    isUsing = false;
                }
            }
            else if (focusedFax)
            {
                if (focusedFax.inUseBy == 0)
                {
                    if (!isUsing)
                    {
                        isUsing = true;
                        lastFax = focusedFax;
                        focusedFax.CmdStartUse(netIdentity.netId);
                    }
                }
                else if (focusedFax.inUseBy == netIdentity.netId)
                {
                    timer += Time.deltaTime;

                    if (timer >= useTime)
                    {
                        isDone = true;
                        focusedFax.CmdFaxdDocument();

                        CmdDestroyDocuments(equipment.primaryClientObject, equipment.primaryObserverObject);
                        equipment.ClearCurrentObject();

                        isUsing = false;
                        focusedFax.CmdStopUse();
                    }

                    useTimerImage.fillAmount = timer / useTime;
                }
                else
                {
                    isUsing = false;
                }
            }
            else
            {
                if (lastShredder)
                {
                    isUsing = false;
                    lastShredder.CmdStopUse();
                    lastShredder = null;
                }

                if (lastFax)
                {
                    isUsing = false;
                    lastFax.CmdStopUse();
                    lastFax = null;
                }

                timer = 0;
                useTimerImage.fillAmount = 0;
                isUsing = false;
            }

        }
        else
        {
            if (lastShredder)
            {
                lastShredder.CmdStopUse();
                lastShredder = null;
            }

            if (lastFax)
            {
                lastFax.CmdStopUse();
                lastFax = null;
            }

            timer = 0;
            useTimerImage.fillAmount = 0;
            isUsing = false;
        }

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
            
            if (waitForPrimaryKeyReleased)
            {
                focusReadout.text = "Faxing";
            }
            else
            {
                focusReadout.text = "Hold " + equipment.primaryUseKey + " to fax documents";
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
