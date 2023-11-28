using Mirror;
using System.Collections;
using UnityEngine;

public class H_GadgetTripwire : H_GadgetBase
{
    [Header("Tripwire Settings")]
    public float setupTime;
    public GameObject tripwirePrefab;
    public GameObject placementPreview;

    [Header("Audio Settings")]
    AudioSource source;
    public AudioClip setupClip;
    public AudioClip placeClip;

    [Header("Viewmodel Settings")]
    public Animator viewmodelAnimator;
    public SkinnedMeshRenderer jacketRenderer;

    Transform placePoint;
    Vector3 finalPlacePoint;
    Quaternion finalRotation;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public override void Initialize()
    {
        base.Initialize();

        placePoint = GetComponentInParent<H_PlayerEquipment>().placePoint;
    }

    public override void Update()
    {
        base.Update();

        if (cooldownTimer > 0)
        {
            placementPreview.SetActive(false);
        }
        else
        {
            placementPreview.transform.position = placePoint.position;
            placementPreview.transform.rotation = placePoint.rotation;

            placementPreview.SetActive(true);
        }

        viewmodelAnimator.SetBool("isOnCooldown", cooldownTimer > 0 ? true : false);

    }

    public override void UseGadgetPrimary()
    {
        if (cooldownTimer <= 0)
        {
            StartCoroutine(SetupTripwire());
            ResetCooldown();
        }
    }

    IEnumerator SetupTripwire()
    {
        viewmodelAnimator.SetTrigger("Set");

        equipment.brain.canMove = false;
        equipment.SetBusy(true);

        CmdSetupTripwireSound();

        yield return new WaitForSeconds(setupTime);

        CmdSetupTripwire();

        equipment.brain.canMove = true;
        equipment.SetBusy(false);

        Debug.Log("Placing tripwire");
    }

    [Command(requiresAuthority = false)]
    public void CmdSetupTripwireSound()
    {
        RpcSetupTripwireSound();
    }

    [ClientRpc]
    public void RpcSetupTripwireSound()
    {
        source.PlayOneShot(setupClip);
    }

    [Command(requiresAuthority = false)]
    public void CmdSetupTripwire()
    {
        RpcSetupTripwire();

        finalPlacePoint = placePoint.position;
        finalRotation = placePoint.rotation;

        GameObject newTripwire = Instantiate(tripwirePrefab, finalPlacePoint, finalRotation);

        NetworkServer.Spawn(newTripwire);

        Debug.Log("Tripwire spawned on server");
    }

    [ClientRpc]
    public void RpcSetupTripwire()
    {
        source.PlayOneShot(placeClip);
    }
}
