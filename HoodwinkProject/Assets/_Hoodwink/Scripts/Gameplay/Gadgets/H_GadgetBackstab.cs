using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

    H_PlayerHealth targetPlayer;

    void Start()
    {
        damageCollider = GetComponent<BoxCollider>();
    }

    public override void Update()
    {
        base.Update();

        viewmodelAnimator.SetBool("isOnCooldown", cooldownTimer > 0 ? true : false);
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

        if (cooldownTimer <= 0 && targetPlayer)
        {
            StartCoroutine(Attack(targetPlayer));
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

    IEnumerator Attack(H_PlayerHealth target)
    {
        H_PlayerHealth newTarget = target;
        PlayStabSuccessLocal();
        CmdPlayStabSuccess();

        viewmodelAnimator.SetBool("hitObject", true);
        viewmodelAnimator.SetTrigger("Stab");

        equipment.brain.SetCanMove(false);
        CmdFreezeTarget(newTarget.brain);

        yield return new WaitForSeconds(attackDelay);

        newTarget.Damage(stabSuccessDamage);

        yield return new WaitForSeconds(attackLength);

        equipment.brain.SetCanMove(true);
        //CmdUnfreezeTarget(newTarget.brain);
    }

    void CmdFreezeTarget(H_PlayerBrain brain)
    {
        brain.RpcSetCanMove(false);
    }

    void CmdUnfreezeTarget(H_PlayerBrain brain)
    {
        brain.RpcSetCanMove(true);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<NetworkIdentity>())
        {
            if (!other.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                var health = other.gameObject.GetComponent<H_PlayerHealth>();

                if (health)
                {
                    if (Vector3.Dot(transform.forward, other.gameObject.transform.forward) >= 0.5)
                    {
                        targetPlayer = health;
                        return;
                    }
                }
            }
        }

        targetPlayer = null;
    }

    public override void UpdateUI()
    {
        if (usePrompt)
        {
            if (cooldownTimer <= 0 && equipment.currentSlot == gadgetSlot && targetPlayer)
            {
                promptReadout.text = prompt;
            }
            else
            {
                promptReadout.text = "";
            }
        }
        else
        {
            promptReadout.text = "";
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
