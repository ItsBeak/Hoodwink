using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class H_DocumentFax : NetworkBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    [Command(requiresAuthority = false)]
    public void CmdFaxdDocument()
    {
        Debug.Log("A document has been faxed");
        H_GameManager.instance.CmdUpdateEvidence(-30);
    }
}
