using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class H_PlayerEquipment : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnEquipmentChanged))]
    public EquipmentSlot currentSlot = EquipmentSlot.Holstered;

    [Header("Primary Equipment Settings")]
    public GameObject primaryClientObject;
    public GameObject primaryObserverObject;

    [Header("Sidearm Equipment Settings")]
    public GameObject sidearmClientObject;
    public GameObject sidearmObserverObject;

    [Header("Holstered Equipment Settings")]
    public GameObject holsteredClientObject;
    public GameObject holsteredObserverObject;

    [Header("Gadget Settings")]
    public Transform gadgetAnchor;
    public H_GadgetBase currentGadget;
    public Image gadgetCooldownUI;
    public Image gadgetIcon;
    public TextMeshProUGUI gadgetNameText;

    [Header("Equipment UI Elements")]
    public Image primarySlotUI;
    public Image sidearmSlotUI;
    public Image holsteredSlotUI;

    public Color selectedColor, deselectedColor;

    [Header("Item Input Settings")]
    public KeyCode interactKey = KeyCode.F;
    public KeyCode primaryUseKey = KeyCode.Mouse0;
    public KeyCode secondaryUseKey = KeyCode.Mouse1;
    public KeyCode alternateUseKey = KeyCode.R;
    public KeyCode gadgetUseKey = KeyCode.G;
    public KeyCode dropKey = KeyCode.Q;

    [HideInInspector] public bool isPrimaryUseKeyPressed = false;
    [HideInInspector] public bool isSecondaryUseKeyPressed = false;
    [HideInInspector] public bool isAlternateUseKeyPressed = false;

    void Start()
    {
        if (!isLocalPlayer)
            return;
        CmdChangeSlot(EquipmentSlot.Holstered);
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        CheckForKeypresses();
        UpdateUI();
    }

    void CheckForKeypresses()
    {
        isPrimaryUseKeyPressed = Input.GetKey(primaryUseKey);
        isSecondaryUseKeyPressed = Input.GetKey(secondaryUseKey);
        isAlternateUseKeyPressed = Input.GetKey(alternateUseKey);

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CmdChangeSlot(EquipmentSlot.PrimaryItem);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CmdChangeSlot(EquipmentSlot.Sidearm);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CmdChangeSlot(EquipmentSlot.Holstered);
        }
        else if (Input.GetKeyDown(gadgetUseKey))
        {
            if (!currentGadget)
            {
                gadgetAnchor.GetComponentInChildren<H_GadgetBase>();
            }

            if (currentGadget)
            {
                currentGadget.UseGadget();
            }
        }
    }

    void UpdateUI()
    {
        if (currentGadget)
        {
            gadgetCooldownUI.fillAmount = Mathf.Clamp(currentGadget.cooldownTimer / currentGadget.cooldown, 0, 1);
            gadgetIcon.sprite = currentGadget.gadgetIcon;
            gadgetIcon.color = Color.white;
            gadgetNameText.text = currentGadget.gadgetName;
        }
        else
        {
            gadgetIcon.sprite = null;
            gadgetIcon.color = Color.clear;
            gadgetNameText.text = "";
        }
    }

    void OnEquipmentChanged(EquipmentSlot oldSlot, EquipmentSlot newSlot)
    {
        ChangeSlot(newSlot);
    }

    void ChangeSlot(EquipmentSlot newSlot)
    {
        primarySlotUI.color = deselectedColor;
        sidearmSlotUI.color = deselectedColor;
        holsteredSlotUI.color = deselectedColor;

        switch (newSlot)
        {
            case EquipmentSlot.PrimaryItem:
                OnSlotPrimary();
                break;

            case EquipmentSlot.Sidearm:
                OnSlotSidearm();
                break;

            case EquipmentSlot.Holstered:
                OnSlotHolstered();
                break;
        }

    }

    [Command]
    void CmdChangeSlot(EquipmentSlot selectedSlot)
    {
        currentSlot = selectedSlot;
    }

    void OnSlotPrimary()
    {
        primaryObserverObject.SetActive(true);
        sidearmObserverObject.SetActive(false);
        holsteredObserverObject.SetActive(false);

        if (!isLocalPlayer)
            return;

        primarySlotUI.color = selectedColor;

        primaryClientObject.SetActive(true);
        sidearmClientObject.SetActive(false);
        holsteredClientObject.SetActive(false);
    }

    void OnSlotSidearm()
    {
        primaryObserverObject.SetActive(false);
        sidearmObserverObject.SetActive(true);
        holsteredObserverObject.SetActive(false);

        if (!isLocalPlayer)
            return;

        sidearmSlotUI.color = selectedColor;

        primaryClientObject.SetActive(false);
        sidearmClientObject.SetActive(true);
        holsteredClientObject.SetActive(false);


    }

    void OnSlotHolstered()
    {
        primaryObserverObject.SetActive(false);
        sidearmObserverObject.SetActive(false);
        holsteredObserverObject.SetActive(true);

        if (!isLocalPlayer)
            return;

        holsteredSlotUI.color = selectedColor;

        primaryClientObject.SetActive(false);
        sidearmClientObject.SetActive(false);
        holsteredClientObject.SetActive(true);

    }
}

public enum EquipmentSlot
{
    PrimaryItem,
    Sidearm,
    Holstered
}