using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class H_DocumentShredder : NetworkBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    [Command(requiresAuthority = false)]
    public void CmdShredDocument()
    {
        Debug.Log("A document has been shredded");
        H_GameManager.instance.CmdUpdateEvidence(10);
    }
}
