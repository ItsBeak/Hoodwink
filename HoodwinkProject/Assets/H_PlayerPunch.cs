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

    public void Start()
    {
        animator = GetComponent<H_PlayerAnimator>();
        equipment = GetComponent<H_PlayerEquipment>();

        attackTimer = attackCooldown;
        damageCollider.enabled = false;
    }

    public void Update()
    {
        if (!isLocalPlayer)
            return;

        attackTimer -= Time.deltaTime;
        canAttack = attackTimer <= 0;

        if (equipment.isPrimaryUseKeyPressed && equipment.currentSlot == EquipmentSlot.PrimaryItem && !equipment.primaryClientObject && canAttack)
        {
            if (H_GameManager.instance.currentRoundStage == RoundStage.Game)
            {
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
        Debug.Log("Punching");
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
                    clientEffects.CmdPlayHit();

                    if (isLocalPlayer)
                    {
                        clientEffects.PlayHitLocal();
                    }

                    health.Damage(attackDamage);

                    damageCollider.enabled = false;
                }
            }
        }
    }
}
