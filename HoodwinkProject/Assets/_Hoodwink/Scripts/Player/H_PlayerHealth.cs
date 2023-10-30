using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using Unity.VisualScripting;

public class H_PlayerHealth : NetworkBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100;

    [SyncVar(hook = nameof(OnHealthChanged))]
    private float currentHealth = 100;

    [SyncVar(hook = nameof(OnDeathStateChanged)), HideInInspector]
    public bool isDead = false;

    [Header("Components")]
    public Image healthBarImage;
    public TextMeshProUGUI textReadout;

    H_PlayerEquipment equipment;
    H_PlayerController controller;
    H_PlayerBrain brain;
    H_GameManager gameManager;
    H_PlayerAnimator animator;
    public H_HealthEffect healthEffects;
    H_Spectating spectating;

    [Header("Hitboxes")]
    public H_PlayerHitbox headHitbox;
    public H_PlayerHitbox[] normalHitboxes;
    public H_PlayerHitbox[] limbHitboxes;


    [Header("Debugging")]
    public bool enableDebugLogs;

    private void Start()
    {
        equipment = GetComponent<H_PlayerEquipment>();
        brain = GetComponent<H_PlayerBrain>();
        animator = GetComponent<H_PlayerAnimator>();
        spectating = GetComponent<H_Spectating>();

        gameManager = FindObjectOfType<H_GameManager>();

        SetupHitboxes();

        currentHealth = maxHealth;
        UpdateUI();
    }

    public void SetupHitboxes()
    {
        headHitbox.Setup(this);
        headHitbox.hitboxType = H_PlayerHitbox.HitboxType.Head;

        foreach (H_PlayerHitbox hit in normalHitboxes)
        {
            hit.Setup(this);
            hit.hitboxType = H_PlayerHitbox.HitboxType.Normal;
        }

        foreach (H_PlayerHitbox hit in limbHitboxes)
        {
            hit.Setup(this);
            hit.hitboxType = H_PlayerHitbox.HitboxType.Limb;
        }
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    private void OnHealthChanged(float oldHealth, float newHealth)
    {
        UpdateUI();

        if (isLocalPlayer)
        {
            if (newHealth > oldHealth)
            {
                healthEffects.Heal();
            }
            else if (newHealth < oldHealth)
            {
                healthEffects.Hit();
            }
        }

        if (enableDebugLogs)
            Debug.Log("Health changed from " + oldHealth + " to " + newHealth + " on player: " + netIdentity.name);
    }

    private void OnDeathStateChanged(bool oldState, bool newState)
    {
        UpdateUI();

        if (isLocalPlayer)
        {
            if (newState)
            {
                spectating.EnableSpectating();
                brain.cosmetics.ShowPlayer();
                brain.equipment.SetDead(newState);
                brain.playerUI.ShowSpectatorUI();
                animator.playerAnimator.SetBool("isDead", true);
                equipment.TryDropItem();
            }
            else
            {
                spectating.DisableSpectating();
                brain.cosmetics.HidePlayer();
                brain.equipment.SetDead(newState);
                brain.playerUI.ShowGameUI();
                animator.playerAnimator.SetBool("isDead", false);
            }
        }
    }

    private void UpdateUI()
    {
        healthBarImage.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
        textReadout.text = isDead || currentHealth == 0 ? "Dead" : currentHealth.ToString() + "/" + maxHealth.ToString();
    }

    private void OnDeath()
    {
        if (enableDebugLogs)
            Debug.Log("Player: " + netIdentity.name + " has died");

        gameManager.CmdPlayerKilled(brain);

    }

    [Command(requiresAuthority = false)]
    public void Damage(int damage)
    {
        float newHealth = currentHealth - damage;
        newHealth = Mathf.Clamp(newHealth, 0, maxHealth);

        currentHealth = newHealth;

        if (currentHealth == 0 && !isDead)
        {
            isDead = true;
            OnDeath();
        }

    }

    [Command(requiresAuthority = false)]
    public void Heal(int amount)
    {
        float newHealth = currentHealth + amount;
        newHealth = Mathf.Min(newHealth, maxHealth);
        currentHealth = newHealth;
    }

   [Command(requiresAuthority = false)]
    public void FullHeal()
    {
        currentHealth = maxHealth;
    }

}
