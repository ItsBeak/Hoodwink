using Mirror;
using UnityEngine;

public class H_GadgetBandage : H_GadgetBase
{

    [Header("Bandage Settings")]
    [SerializeField] int healAmount;
    H_PlayerHealth playerHealth;

    public LayerMask targetLayers;

    public override void CmdUseGadgetPrimary()
    {
        base.CmdUseGadgetPrimary();

        playerHealth = GetComponentInParent<H_PlayerHealth>();

        Debug.Log("Heal");
        playerHealth.Heal(healAmount);
    }

    public override void CmdUseGadgetSecondary()
    {
        base.CmdUseGadgetSecondary();

        RaycastHit hit;

        if (Physics.Raycast(equipment.playerCamera.transform.position, transform.forward, out hit, 2, targetLayers))
        {

            if (!hit.collider.transform.IsChildOf(equipment.transform))
            {
                var health = hit.collider.gameObject.GetComponentInParent<H_PlayerHealth>();

                if (health)
                {
                    health.Heal(healAmount);
                }
            }
            else
            {
                Debug.LogError("Player heal hit " + hit.collider.gameObject.name + ", which is a child of this player. Check layermask settings on both the hit object and the target player");
            }
        }

    }
}
