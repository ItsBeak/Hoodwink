using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_Tripwire : NetworkBehaviour
{
    [Header("Tripwire Settings")]
    public float explosionRadius;
    public int explosionDamage;
    public LayerMask playerLayers;

    AudioSource source;
    public AudioClip triggerClip;
    public ParticleSystem explosionParticle;

    [SyncVar]
    bool isTriggered = false;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            H_PlayerBrain player = other.GetComponent<H_PlayerBrain>();

            if (player)
            {
                if (player.currentAlignment != AgentAlignment.Spy)
                {
                    if (!isTriggered)
                    {
                        CmdTriggerTripwire();
                    }
                }
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdTriggerTripwire()
    {
        isTriggered = true;
         
        RpcTriggerTripwire();

        foreach (Collider hitPlayer in Physics.OverlapSphere(transform.position, explosionRadius, playerLayers))
        {
            H_PlayerBrain player = hitPlayer.GetComponent<H_PlayerBrain>();

            if (player)
            {
                Debug.Log("Tripwire has been set off by " + player.name);
                player.GetComponent<H_PlayerHealth>().Damage(explosionDamage);
            }
        }
    }

    [ClientRpc]
    public void RpcTriggerTripwire()
    {
        source.PlayOneShot(triggerClip);
        explosionParticle.Play();
        GetComponent<LineRenderer>().enabled = false;
    }

}
