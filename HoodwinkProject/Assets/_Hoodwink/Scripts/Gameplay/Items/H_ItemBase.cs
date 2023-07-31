using UnityEngine;
using Mirror;
using Unity.VisualScripting;

public class H_ItemBase : NetworkBehaviour
{
    [HideInInspector] public H_PlayerEquipment equipment;

    public GameObject worldDropItem;
    public bool dropOnSwap;
    [SerializeField] SlotType slotType;

    public string itemName;
    public Sprite itemIcon;

    public bool usePrimaryRepeating;
    public float primaryUseRate;
    [HideInInspector] public float timeUntilNextPrimaryUse;
    [HideInInspector] public bool waitForPrimaryKeyReleased = false;

    public bool useSecondaryRepeating;
    public float secondaryUseRate;
    [HideInInspector] public float timeUntilNextSecondaryUse;
    [HideInInspector] public bool waitForSecondaryKeyReleased = false;

    public bool useAlternateRepeating;
    public float alternateUseRate;
    [HideInInspector] public float timeUntilNextAlternateUse;
    [HideInInspector] public bool waitForAlternateKeyReleased = false;

    public virtual void Update()
    {
        if (!isOwned)
            return;

        CheckForKeyPresses();
    }

    public virtual void Initialize()
    {
        if (!equipment)
        {
            equipment = GetComponentInParent<H_PlayerEquipment>();
        }

        if (slotType == SlotType.Primary)
        {
            equipment.primaryItemName.text = itemName;
            equipment.primaryItemIcon.sprite = itemIcon;
            equipment.primaryItemIcon.color = Color.white;
        }
        else if (slotType == SlotType.Sidearm)
        {
            equipment.sidearmItemName.text = itemName;
            equipment.sidearmItemIcon.sprite = itemIcon;
            equipment.sidearmItemIcon.color = Color.white;
        }

    }

    void CheckForKeyPresses()
    {

        if (!equipment)
        {
            equipment = GetComponentInParent<H_PlayerEquipment>();
            return;
        }

        #region Primary Use Key

        timeUntilNextPrimaryUse -= 1 * Time.deltaTime;



        if (equipment.isPrimaryUseKeyPressed)
        {

            if (usePrimaryRepeating)
            {
                if (timeUntilNextPrimaryUse <= 0)
                {
                    PrimaryUse();
                    timeUntilNextPrimaryUse = primaryUseRate;
                }
            }
            else
            {
                if (timeUntilNextPrimaryUse <= 0)
                {
                    if (waitForPrimaryKeyReleased == false)
                    {
                        waitForPrimaryKeyReleased = true;
                        PrimaryUse();
                        timeUntilNextPrimaryUse = primaryUseRate;
                    }
                }
            }
        }
        else
        {
            waitForPrimaryKeyReleased = false;
        }

        #endregion

        #region Secondary Use Key

        timeUntilNextSecondaryUse -= 1 * Time.deltaTime;

        if (equipment.isSecondaryUseKeyPressed)
        {

            if (useSecondaryRepeating)
            {
                if (timeUntilNextSecondaryUse <= 0)
                {
                    SecondaryUse();
                    timeUntilNextSecondaryUse = secondaryUseRate;
                }
            }
            else
            {
                if (timeUntilNextSecondaryUse <= 0)
                {
                    if (waitForSecondaryKeyReleased == false)
                    {
                        waitForSecondaryKeyReleased = true;
                        SecondaryUse();
                        timeUntilNextSecondaryUse = secondaryUseRate;
                    }
                }
            }
        }
        else
        {
            waitForSecondaryKeyReleased = false;
        }

        #endregion

        #region Alternate Use Key

        timeUntilNextAlternateUse -= 1 * Time.deltaTime;

        if (equipment.isAlternateUseKeyPressed)
        {

            if (useAlternateRepeating)
            {
                if (timeUntilNextAlternateUse <= 0)
                {
                    AlternateUse();
                    timeUntilNextAlternateUse = alternateUseRate;
                }
            }
            else
            {
                if (timeUntilNextAlternateUse <= 0)
                {
                    if (waitForAlternateKeyReleased == false)
                    {
                        waitForAlternateKeyReleased = true;
                        AlternateUse();
                        timeUntilNextAlternateUse = alternateUseRate;
                    }
                }
            }
        }
        else
        {
            waitForAlternateKeyReleased = false;
        }

        #endregion
    }

    public virtual void PrimaryUse()
    {
        CmdPrimaryUse();
    }

    public virtual void SecondaryUse()
    {
        CmdSecondaryUse();
    }

    public virtual void AlternateUse()
    {
        CmdAlternateUse();
    }

    [Command(requiresAuthority = false)]
    public virtual void CmdPrimaryUse()
    {
        RpcPrimaryUse();
    }

    [Command(requiresAuthority = false)]
    public virtual void CmdSecondaryUse()
    {
        RpcSecondaryUse();
    }

    [Command(requiresAuthority = false)]
    public virtual void CmdAlternateUse()
    {
        RpcAlternateUse();
    }

    [ClientRpc]
    public virtual void RpcPrimaryUse()
    {

    }

    [ClientRpc]
    public virtual void RpcSecondaryUse()
    {

    }

    [ClientRpc]
    public virtual void RpcAlternateUse()
    {

    }
}

enum SlotType
{
    Primary,
    Sidearm
}
