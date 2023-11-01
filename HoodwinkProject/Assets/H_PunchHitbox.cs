using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class H_PunchHitbox : MonoBehaviour
{
    H_PlayerPunch punch;

    private void Start()
    {
        punch = GetComponentInParent<H_PlayerPunch>();
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject hitObject = other.gameObject;

        if (hitObject.GetComponent<NetworkIdentity>())
        {
            if (!hitObject.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                var health = hitObject.GetComponent<H_PlayerHealth>();

                if (health)
                {
                    punch.Hit(health);
                }
            }
        }
    }
}
