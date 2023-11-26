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

    [Header("Animation Timings")]
    public float pinDelay;
    public float dropDelay;

    [Header("Viewmodel Settings")]
    public Animator viewmodelAnimator;
    public SkinnedMeshRenderer jacketRenderer;

    AudioSource source;

    public override void Initialize()
    {
        base.Initialize();

        jacketRenderer.material.color = equipment.GetComponent<H_PlayerBrain>().agentData.primaryColour;

        source = GetComponent<AudioSource>();
    }

    public override void Update()
    {
        base.Update();

        viewmodelAnimator.SetBool("isOnCooldown", cooldownTimer > 0 ? true : false);
    }

    public override void UseGadgetPrimary()
    {

        if (cooldownTimer <= 0)
        {
            StartCoroutine(Drop());

            cooldownTimer = cooldown;
        }
    }

    IEnumerator Drop()
    {
        viewmodelAnimator.SetTrigger("Throw");

        yield return new WaitForSeconds(pinDelay);

        source.Play();

        yield return new WaitForSeconds(dropDelay);

        CmdUseGadgetPrimary();
    }

    [Command(requiresAuthority = false)]
    public override void CmdUseGadgetPrimary()
    {
        base.CmdUseGadgetPrimary();

        Transform dropPoint = GetComponentInParent<H_PlayerEquipment>().dropPoint;

        Vector3 position = equipment.transform.position + new Vector3(0, 0.2f, 0);
        Quaternion rotation = Quaternion.identity;
        GameObject smokeGrenade = Instantiate(smokeGrenadePrefab, position, rotation);

        smokeGrenade.GetComponent<Rigidbody>().AddTorque(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
        smokeGrenade.GetComponent<Rigidbody>().AddForce(dropPoint.forward * throwForce, ForceMode.Impulse);

        NetworkServer.Spawn(smokeGrenade);
    }
}
