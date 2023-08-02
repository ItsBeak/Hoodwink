using UnityEngine;
using Mirror;
using System.Collections;

using System;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BoxCollider))]
public class H_ItemMelee : H_ItemBase
{
    public int attackDamage = 1;
    public float attackLength;
    public float attackDelay;
    public float attackCooldown;

    float attackTimer;
    bool canAttack;

    [Header("Effects")]
    H_MeleeEffects clientEffects;
    H_MeleeEffects observerEffects;

    BoxCollider damageCollider;
    H_PlayerAnimator animator;

    public override void Initialize()
    {
        base.Initialize();

        clientEffects = GetComponent<H_MeleeEffects>();
        observerEffects = equipment.holsteredEquipPointObserver.GetComponentInChildren<H_MeleeEffects>();
        animator = equipment.transform.GetComponent<H_PlayerAnimator>();

        damageCollider = GetComponent<BoxCollider>();
        attackTimer = attackCooldown;
        damageCollider.enabled = false;

    }

    public override void Update()
    {
        base.Update();

        attackTimer -= Time.deltaTime;
        canAttack = attackTimer <= 0;
    }

    public override void PrimaryUse()
    {
        base.PrimaryUse();

        if (canAttack)
        {
            StartCoroutine(Attack());
            attackTimer = attackCooldown;
            animator.CmdPlayPunchAnimation();
            clientEffects.PlaySwingLocal();
            observerEffects.CmdPlaySwing();

        }
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
                    clientEffects.PlayHitLocal();
                    observerEffects.CmdPlayHit();

                    health.Damage(attackDamage);

                    damageCollider.enabled = false;
                }
            }
        }
    }
}
