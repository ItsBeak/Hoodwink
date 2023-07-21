using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class H_PlayerEquipment : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnEquipmentChanged))]
    public EquipmentSlot currentSlot = EquipmentSlot.Holstered;

    GameObject currentSlotObject;

    [Header("Primary Equipment Settings")]
    public GameObject itemAnchor;
    public GameObject primaryObject;

    [Header("Sidearm Equipment Settings")]
    public GameObject sidearmObject;

    [Header("Holstered Equipment Settings")]
    public GameObject holsteredObject;

    [Header("Equipment UI Elements")]
    public Image primarySlotUI;
    public Image sidearmSlotUI;
    public Image holsteredSlotUI;

    public Color selectedColor, deselectedColor;

    void Start()
    {

    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        CheckForKeypresses();
    }

    void CheckForKeypresses()
    {
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
        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (currentSlot == EquipmentSlot.PrimaryItem)
            {

            }
            else if (currentSlot == EquipmentSlot.Sidearm)
            {
                currentSlotObject.GetComponent<H_ThingDoer>().DoThings();
            }
            else if (currentSlot == EquipmentSlot.Holstered)
            {

            }
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
        primarySlotUI.color = selectedColor;

        primaryObject.SetActive(true);
        sidearmObject.SetActive(false);
        holsteredObject.SetActive(false);

        currentSlotObject = primaryObject;
    }

    void OnSlotSidearm()
    {
        sidearmSlotUI.color = selectedColor;

        primaryObject.SetActive(false);
        sidearmObject.SetActive(true);
        holsteredObject.SetActive(false);

        currentSlotObject = sidearmObject;
    }

    void OnSlotHolstered()
    {
        holsteredSlotUI.color = selectedColor;

        primaryObject.SetActive(false);
        sidearmObject.SetActive(false);
        holsteredObject.SetActive(true);

        currentSlotObject = holsteredObject;
    }
}

public enum EquipmentSlot
{
    PrimaryItem,
    Sidearm,
    Holstered
}