using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_PlayerPunch : NetworkBehaviour
{
    public int attackDamage = 1;
    public float attackLength;
    public float attackDelay;
    public float attackCooldown;

    float attackTimer;
    bool canAttack;

    [Header("Effects")]
    public H_MeleeEffects clientEffects;

    public BoxCollider damageCollider;
    H_PlayerAnimator animator;
    H_PlayerEquipment equipment;
    H_PlayerHealth health;

    RoundStage stage;

    public void Start()
    {
        animator = GetComponent<H_PlayerAnimator>();
        equipment = GetComponent<H_PlayerEquipment>();
        health = GetComponent<H_PlayerHealth>();

        attackTimer = attackCooldown;
        damageCollider.enabled = false;
    }

    public void Update()
    {
        if (!isLocalPlayer)
            return;

        attackTimer -= Time.deltaTime;
        canAttack = attackTimer <= 0;

        stage = H_GameManager.instance.currentRoundStage;

        if (equipment.isPrimaryUseKeyPressed && equipment.currentSlot == EquipmentSlot.PrimaryItem && !equipment.primaryClientObject && canAttack && !health.isDead)
        {
            if (stage == RoundStage.Game || stage == RoundStage.Lobby)
            {
                GetComponent<H_PlayerAnimator>().jacketRenderer.material.color = equipment.brain.agentData.primaryColour;

                StartCoroutine(Attack());
                attackTimer = attackCooldown;
                animator.CmdPlayPunchAnimation();
                clientEffects.CmdPlaySwing();

                if (isLocalPlayer)
                {
                    clientEffects.PlaySwingLocal();
                }
            }
        }
    }

    IEnumerator Attack()
    {
        animator.fistsAnimator.SetBool("hitObject", false);

        animator.fistsAnimator.SetTrigger("Punch");
        Debug.Log("Punching");

        yield return new WaitForSeconds(attackDelay);

        damageCollider.enabled = true;

        yield return new WaitForSeconds(attackLength);

        damageCollider.enabled = false;
    }

    public void Hit(H_PlayerHealth health)
    {
        clientEffects.CmdPlayHit();

        if (isLocalPlayer)
        {
            clientEffects.PlayHitLocal();
        }

        if (stage == RoundStage.Game)
        {
            health.Damage(attackDamage);
        }

        animator.fistsAnimator.SetBool("hitObject", true);

        damageCollider.enabled = false;
    }
}
