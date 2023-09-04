using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class H_GadgetSmokeGrenade : H_GadgetBase
{

    [Header("Smoke Settings")]
    [SerializeField] GameObject smokeGrenadePrefab;
    [SerializeField] float throwForce = 1;

    public override void CmdUseGadgetPrimary()
    {
        base.CmdUseGadgetPrimary();

        Transform dropPoint = GetComponentInParent<H_PlayerEquipment>().dropPoint;

        Vector3 position = dropPoint.position;
        Quaternion rotation = dropPoint.rotation;
        GameObject smokeGrenade = Instantiate(smokeGrenadePrefab, position, rotation);

        smokeGrenade.GetComponent<Rigidbody>().AddTorque(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
        smokeGrenade.GetComponent<Rigidbody>().AddForce(dropPoint.forward * throwForce, ForceMode.Impulse);

        NetworkServer.Spawn(smokeGrenade);
    }
}
