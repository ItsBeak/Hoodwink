using UnityEngine;
using Mirror;
using TMPro;

public class H_GadgetHealthMonitor : H_GadgetBase
{
    public LayerMask targetLayers;

    public TextMeshProUGUI healthReadout;

    public override void CmdUseGadgetPrimary()
    {
        base.CmdUseGadgetPrimary();

        RaycastHit hit;

        if (Physics.Raycast(equipment.playerCamera.transform.position, transform.forward, out hit, 2, targetLayers))
        {

            if (!hit.collider.transform.IsChildOf(equipment.transform))
            {
                var health = hit.collider.gameObject.GetComponentInParent<H_PlayerHealth>();

                if (health)
                {
                    ShowText();

                    Debug.Log("Players health is " + health.GetHealth());

                    healthReadout.text = "Health: " + health.GetHealth();

                    Invoke(nameof(HideText), 2f);
                }
            }
            else
            {
                Debug.LogError("Health monitor has targeted " + hit.collider.gameObject.name + ", which is a child of this player. Check layermask settings on both the hit object and the target player");
            }
        }
    }

    public override void UseGadgetSecondary()
    {
        
    }

    void ShowText()
    {
        healthReadout.color = Color.red;
    }

    void HideText()
    {
        healthReadout.color = Color.clear;
    }
}
