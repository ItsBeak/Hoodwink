using Cinemachine;
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
    public Transform primaryEquipPointClient;
    public Transform primaryEquipPointObserver;
    [HideInInspector] public GameObject primaryClientObject;
    [HideInInspector] public GameObject primaryObserverObject;

    [Header("Sidearm Equipment Settings")]
    public Transform sidearmEquipPointClient;
    public Transform sidearmEquipPointObserver;
    [HideInInspector] public GameObject sidearmClientObject;
    [HideInInspector] public GameObject sidearmObserverObject;

    [Header("Holstered Equipment Settings")]
    public Transform holsteredEquipPointClient;
    public Transform holsteredEquipPointObserver;
    [HideInInspector] public GameObject holsteredClientObject;
    [HideInInspector] public GameObject holsteredObserverObject;

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

    [Header("Hit Markers")]
    public Transform hitmarkerParent;
    public GameObject hitmarkerPrefab;

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

    [Header("Components")]
    public CinemachineVirtualCamera playerCamera;
    [HideInInspector] public float baseFOV;


    void Start()
    {
        if (!isLocalPlayer)
        {
            primaryEquipPointClient.gameObject.SetActive(false);
            sidearmEquipPointClient.gameObject.SetActive(false);
            holsteredEquipPointClient.gameObject.SetActive(false);
            return;
        }

        primaryEquipPointObserver.gameObject.SetActive(false);
        sidearmEquipPointObserver.gameObject.SetActive(false);
        holsteredEquipPointObserver.gameObject.SetActive(false);

        baseFOV = playerCamera.m_Lens.FieldOfView;

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
            gadgetCooldownUI.fillAmount = 0;
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
        ClearSlots();

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

        playerCamera.m_Lens.FieldOfView = baseFOV;

    }

    [Command]
    void CmdChangeSlot(EquipmentSlot selectedSlot)
    {
        currentSlot = selectedSlot;
    }

    void ClearSlots()
    {
        primaryEquipPointObserver.gameObject.SetActive(false);
        sidearmEquipPointObserver.gameObject.SetActive(false);
        holsteredEquipPointObserver.gameObject.SetActive(false);

        if (!isLocalPlayer)
            return;

        primaryEquipPointClient.gameObject.SetActive(false);
        sidearmEquipPointClient.gameObject.SetActive(false);
        holsteredEquipPointClient.gameObject.SetActive(false);
    }

    void OnSlotPrimary()
    {

        if (!isLocalPlayer)
        {
            primaryEquipPointObserver.gameObject.SetActive(true);
        }
        else
        {
            primarySlotUI.color = selectedColor;

            primaryEquipPointClient.gameObject.SetActive(true);
        }
    }

    void OnSlotSidearm()
    {

        if (!isLocalPlayer)
        {
            sidearmEquipPointObserver.gameObject.SetActive(true);
        }
        else
        {
            sidearmSlotUI.color = selectedColor;

            sidearmEquipPointClient.gameObject.SetActive(true);
        }
    }

    void OnSlotHolstered()
    {
        if (!isLocalPlayer)
        {
            holsteredEquipPointObserver.gameObject.SetActive(true);
        }
        else
        {
            holsteredSlotUI.color = selectedColor;

            holsteredEquipPointClient.gameObject.SetActive(true);
        }
    }

    [ClientRpc]
    public void RpcEquipSidearm(GameObject clientSidearm, GameObject observerSidearm)
    {
        sidearmClientObject = clientSidearm;
        sidearmObserverObject = observerSidearm;

        sidearmClientObject.transform.parent = sidearmEquipPointClient;
        sidearmClientObject.transform.localPosition = Vector3.zero;
        sidearmClientObject.transform.localRotation = Quaternion.identity;

        sidearmObserverObject.transform.parent = sidearmEquipPointObserver;
        sidearmObserverObject.transform.localPosition = Vector3.zero;
        sidearmObserverObject.transform.localRotation = Quaternion.identity;

        clientSidearm.GetComponent<H_ItemWeapon>().Initialize();
    }

    [ClientRpc]
    public void RpcEquipGadget(GameObject gadget)
    {
        currentGadget = gadget.GetComponent<H_GadgetBase>();

        currentGadget.transform.parent = gadgetAnchor;
        currentGadget.transform.localPosition = Vector3.zero;
        currentGadget.transform.localRotation = Quaternion.identity;
    }

    public void SpawnHitMarker()
    {
        Instantiate(hitmarkerPrefab, hitmarkerParent);
    }


}

public enum EquipmentSlot
{
    PrimaryItem,
    Sidearm,
    Holstered
}