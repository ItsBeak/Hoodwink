using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class H_GadgetBackstab : H_GadgetBase
{
    [Header("Backstab Settings")]
    public int stabSuccessDamage = 50;
    public int stabFailDamage = 5;

    public float attackLength;
    public float attackDelay;

    BoxCollider damageCollider;
    H_PlayerAnimator animator;

    [Header("Audio Settings")]
    public AudioClip[] swingClips;
    public AudioClip[] stabSuccessClips;
    public AudioClip[] stabFailClips;
    public AudioSource source;

    void Start()
    {
        damageCollider = GetComponent<BoxCollider>();
        damageCollider.enabled = false;
    }

    public override void UseGadget()
    {

        if (!animator)
        {
            animator = GetComponentInParent<H_PlayerAnimator>();
        }

        if (cooldownTimer <= 0)
        {
            StartCoroutine(Attack());
            animator.CmdPlayStabAnimation();

            PlaySwingLocal();
            CmdPlaySwing();

            CmdUseGadget();
            cooldownTimer = cooldown;
        }
    }

    public override void RpcUseGadget()
    {
        source.PlayOneShot(swingClips[Random.Range(0, swingClips.Length)]);
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(attackDelay);
        damageCollider.enabled = true;
        yield return new WaitForSeconds(attackLength);
        damageCollider.enabled = false;
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
                    if (Vector3.Dot(transform.forward, hitObject.transform.forward) >= 0.5)
                    {
                        PlayStabSuccessLocal();
                        CmdPlayStabSuccess();

                        health.Damage(stabSuccessDamage);

                        damageCollider.enabled = false;
                    }
                    else
                    {
                        PlayStabFailLocal();
                        CmdPlayStabFail();

                        health.Damage(stabFailDamage);

                        damageCollider.enabled = false;
                    }
                }
            }
        }
    }

    public void PlaySwingLocal()
    {
        source.PlayOneShot(swingClips[Random.Range(0, swingClips.Length)]);
    }

    [Command(requiresAuthority = false)]
    public void CmdPlaySwing()
    {
        RpcPlaySwing();
    }

    [ClientRpc(includeOwner = false)]
    void RpcPlaySwing()
    {
        source.PlayOneShot(swingClips[Random.Range(0, swingClips.Length)]);
    }

    public void PlayStabSuccessLocal()
    {
        source.PlayOneShot(stabSuccessClips[Random.Range(0, stabSuccessClips.Length)]);
    }

    [Command(requiresAuthority = false)]
    public void CmdPlayStabSuccess()
    {
        RpcPlayStabSuccess();
    }

    [ClientRpc(includeOwner = false)]
    void RpcPlayStabSuccess()
    {
        source.PlayOneShot(stabSuccessClips[Random.Range(0, stabSuccessClips.Length)]);
    }

    public void PlayStabFailLocal()
    {
        source.PlayOneShot(stabFailClips[Random.Range(0, stabFailClips.Length)]);
    }

    [Command(requiresAuthority = false)]
    public void CmdPlayStabFail()
    {
        RpcPlayStabFail();
    }

    [ClientRpc(includeOwner = false)]
    void RpcPlayStabFail()
    {
        source.PlayOneShot(stabFailClips[Random.Range(0, stabFailClips.Length)]);
    }


}
