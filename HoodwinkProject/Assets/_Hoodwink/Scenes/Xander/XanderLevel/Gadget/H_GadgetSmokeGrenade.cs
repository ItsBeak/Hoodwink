using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class H_GadgetSmokeGrenade : H_GadgetBase
{

    [Header("Smoke Settings")]
    [SerializeField] GameObject smokeParticles;


    public override void UseGadget()
    {

        Vector3 newpos = gameObject.transform.position;


        Instantiate(smokeParticles, newpos, Quaternion.identity);
    }
}
