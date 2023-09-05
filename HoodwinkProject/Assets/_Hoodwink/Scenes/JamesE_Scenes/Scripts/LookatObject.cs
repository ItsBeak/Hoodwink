using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookatObject : MonoBehaviour
{
    //Box which the empty object which has the script is within, which should be rotating. 
    public GameObject HostBox;
    //Object which the box should be facing.
    public Transform OtherBoxCentre;

    private void Update()
    {
        HostBox.transform.LookAt(OtherBoxCentre);
    }
}
