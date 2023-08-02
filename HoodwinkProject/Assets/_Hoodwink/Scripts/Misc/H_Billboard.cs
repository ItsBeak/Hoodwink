using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_Billboard : MonoBehaviour
{
    Transform mainCam;

    void Start()
    {
        mainCam = Camera.main.transform;
    }

    void Update()
    {
        transform.LookAt(mainCam);    
    }
}
