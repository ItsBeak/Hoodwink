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

    public GameObject primarySlotPrefab;
    public GameObject sidearmSlotPrefab;
    public GameObject holsteredSlotPrefab;

    public GameObject currentSlotObject;

    public GameObject itemAnchor;

    [Header("UI Elements")]
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
            if (currentSlot == EquipmentSlot.Sidearm)
            {
                currentSlotObject.GetComponent<H_ThingDoer>().DoThings();
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
                primarySlotUI.color = selectedColor;

                primarySlotPrefab.SetActive(true);
                sidearmSlotPrefab.SetActive(false);
                holsteredSlotPrefab.SetActive(false);

                currentSlotObject = primarySlotPrefab;
                break;

            case EquipmentSlot.Sidearm:
                sidearmSlotUI.color = selectedColor;

                primarySlotPrefab.SetActive(false);
                sidearmSlotPrefab.SetActive(true);
                holsteredSlotPrefab.SetActive(false);

                currentSlotObject = sidearmSlotPrefab;
                break;

            case EquipmentSlot.Holstered:
                holsteredSlotUI.color = selectedColor;

                primarySlotPrefab.SetActive(false);
                sidearmSlotPrefab.SetActive(false);
                holsteredSlotPrefab.SetActive(true);

                currentSlotObject = holsteredSlotPrefab;
                break;
        }

    }

    [Command] 
    void CmdChangeSlot(EquipmentSlot selectedSlot)
    {
        currentSlot = selectedSlot;
    }

}

public enum EquipmentSlot
{
    PrimaryItem,
    Sidearm,
    Holstered
}