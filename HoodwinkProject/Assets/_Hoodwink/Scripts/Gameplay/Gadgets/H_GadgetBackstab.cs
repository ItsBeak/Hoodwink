using Mirror;
using System.Collections;
using UnityEngine;

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

    [Header("Viewmodel Settings")]
    public Animator viewmodelAnimator;
    public SkinnedMeshRenderer jacketRenderer;

    void Start()
    {
        damageCollider = GetComponent<BoxCollider>();
        damageCollider.enabled = false;
    }

    public override void Update()
    {
        base.Update();

        viewmodelAnimator.SetBool("isOnCooldown", cooldownTimer > 0 ? true : false);

        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log(cooldownTimer);
        }
    }

    public override void Initialize()
    {
        base.Initialize();

        jacketRenderer.material.color = equipment.GetComponent<H_PlayerBrain>().agentData.primaryColour;
    }

    public override void UseGadgetPrimary()
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

            CmdUseGadgetPrimary();
            cooldownTimer = cooldown;
        }
    }

    public override void UseGadgetSecondary()
    {

    }

    public override void RpcUseGadgetPrimary()
    {
        source.PlayOneShot(swingClips[Random.Range(0, swingClips.Length)]);
    }

    IEnumerator Attack()
    {
        viewmodelAnimator.SetBool("hitObject", false);

        viewmodelAnimator.SetTrigger("Stab");

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

                        viewmodelAnimator.SetBool("hitObject", true);

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
