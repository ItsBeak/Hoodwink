using Mirror;
using System.Collections;
using UnityEngine;

public class H_GadgetTripwire : H_GadgetBase
{
    [Header("Tripwire Settings")]
    public float setupTime;
    public GameObject tripwirePrefab;

    AudioSource source;

    public AudioClip setupClip;
    public AudioClip placeClip;


    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public override void UseGadgetPrimary()
    {
        if (cooldownTimer <= 0)
        {
            StartCoroutine(SetupTripwire());
            ResetCooldown();
        }
    }

    public override void UseGadgetSecondary()
    {
        
    }

    IEnumerator SetupTripwire()
    {
        equipment.brain.canMove = false;
        CmdSetupTripwireSound();

        yield return new WaitForSeconds(setupTime);

        equipment.brain.canMove = true;
        CmdSetupTripwire();
    }

    [Command]
    public void CmdSetupTripwireSound()
    {
        RpcSetupTripwireSound();
    }

    [ClientRpc]
    public void RpcSetupTripwireSound()
    {
        source.PlayOneShot(setupClip);
    }

    [Command]
    public void CmdSetupTripwire()
    {
        RpcSetupTripwire();

        Transform placePoint = GetComponentInParent<H_PlayerEquipment>().placePoint;

        Vector3 position = placePoint.position;
        Quaternion rotation = placePoint.rotation;
        GameObject newTripwire = Instantiate(tripwirePrefab, position, rotation);

        NetworkServer.Spawn(newTripwire);
    }

    [ClientRpc]
    public void RpcSetupTripwire()
    {
        source.PlayOneShot(placeClip);
    }
}
