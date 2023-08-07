using Cinemachine;
using Mirror;
using TMPro;
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
    public Image primaryItemIcon;
    [HideInInspector, SyncVar] public bool isHoldingItem = false;
    H_ItemBase currentObject;
    public Transform dropPoint;

    [Header("Sidearm Equipment Settings")]
    public Transform sidearmEquipPointClient;
    public Transform sidearmEquipPointObserver;
    [HideInInspector] public GameObject sidearmClientObject;
    [HideInInspector] public GameObject sidearmObserverObject;
    public Image sidearmItemIcon;

    [Header("Holstered Equipment Settings")]
    public Transform holsteredEquipPointClient;
    public Transform holsteredEquipPointObserver;
    [HideInInspector] public GameObject holsteredClientObject;
    [HideInInspector] public GameObject holsteredObserverObject;
    public Image holsteredItemIcon;

    [Header("Gadget Settings")]
    public Transform gadgetAnchor;
    public H_GadgetBase currentGadget;
    public Image gadgetCooldownUI;
    public Image gadgetIcon;
    public TextMeshProUGUI gadgetNameText;
    public TextMeshProUGUI gadgetDescriptionText;

    [Header("Equipment UI Elements")]
    public Image primarySlotUI;
    public Image sidearmSlotUI;
    public Image holsteredSlotUI;

    [Header("Ammo UI Elements")]
    public Image reloadingImage;
    public TextMeshProUGUI ammoLoadedText, ammoPoolText;

    [Header("Interaction Settings")]
    public float interactionRange = 2f;
    public TextMeshProUGUI focusedItemReadout;
    public LayerMask interactableLayers;
    H_WorldItem focusedItem;

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
    public H_Recoil cameraRecoil;
    H_PlayerBrain brain;
    [HideInInspector] public H_PlayerAnimator animator;
    bool isDead;

    void Start()
    {
        if (!isLocalPlayer)
        {
            primaryEquipPointClient.gameObject.SetActive(false);
            sidearmEquipPointClient.gameObject.SetActive(false);
            holsteredEquipPointClient.gameObject.SetActive(false);
            return;
        }

        brain = GetComponent<H_PlayerBrain>();
        animator = GetComponent<H_PlayerAnimator>();

        primaryEquipPointObserver.gameObject.SetActive(false);
        sidearmEquipPointObserver.gameObject.SetActive(false);
        holsteredEquipPointObserver.gameObject.SetActive(false);

        baseFOV = playerCamera.m_Lens.FieldOfView;

        ChangeSlotInput(EquipmentSlot.Holstered);
    }

    void Update()
    {
        if (!isLocalPlayer || isDead)
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
            ChangeSlotInput(EquipmentSlot.PrimaryItem);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeSlotInput(EquipmentSlot.Sidearm);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeSlotInput(EquipmentSlot.Holstered);
        }
        else if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
        else if (Input.GetKeyDown(dropKey))
        {
            TryDropItem();
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
            gadgetDescriptionText.text = currentGadget.gadgetDescription;
        }
        else
        {
            gadgetIcon.sprite = null;
            gadgetIcon.color = Color.clear;
            gadgetCooldownUI.fillAmount = 0;
            gadgetNameText.text = "";
            gadgetDescriptionText.text = "";
        }

        if (focusedItem && !isHoldingItem)
        {
            focusedItemReadout.text = "Press " + interactKey + " to pickup " + focusedItem.itemName;
        }
        else
        {
            focusedItemReadout.text = "";
        }
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        RaycastHit hit;

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionRange, interactableLayers) && !isHoldingItem)
        {
            H_WorldItem item = hit.collider.GetComponent<H_WorldItem>();

            if (item != null)
            {
                focusedItem = item;
            }
            else
            {
                focusedItem = null;
            }
        }
        else
        {
            focusedItem = null;
        }
    }

    void OnEquipmentChanged(EquipmentSlot oldSlot, EquipmentSlot newSlot)
    {
        ChangeSlot(newSlot);
    }

    void ChangeSlot(EquipmentSlot newSlot)
    {
        ClearSlots();
        ClearAmmoUI();

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

    void ChangeSlotInput(EquipmentSlot selectedSlot)
    {
        if (isHoldingItem)
        {
            if (currentObject.dropOnSwap)
            {
                TryDropItem();
            }
        }

        CmdChangeSlot(selectedSlot);
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
            primaryEquipPointClient.gameObject.SetActive(true);

            brain.playerUI.slotPrimaryAnimator.SetBool("HotBar 1", true);
            brain.playerUI.slotSidearmAnimator.SetBool("HotBar 2", false);
            brain.playerUI.slotHolsteredAnimator.SetBool("HotBar 3", false);
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
            sidearmEquipPointClient.gameObject.SetActive(true);

            brain.playerUI.slotPrimaryAnimator.SetBool("HotBar 1", false);
            brain.playerUI.slotSidearmAnimator.SetBool("HotBar 2", true);
            brain.playerUI.slotHolsteredAnimator.SetBool("HotBar 3", false);
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
            holsteredEquipPointClient.gameObject.SetActive(true);

            brain.playerUI.slotPrimaryAnimator.SetBool("HotBar 1", false);
            brain.playerUI.slotSidearmAnimator.SetBool("HotBar 2", false);
            brain.playerUI.slotHolsteredAnimator.SetBool("HotBar 3", true);
        }
    }

    public void TryInteract()
    {
        if (focusedItem != null)
        {
            CmdTryPickUpItem(focusedItem.netIdentity);
            CmdChangeSlot(EquipmentSlot.PrimaryItem);
        }
    }

    public void TryDropItem()
    {
        if (isHoldingItem)
        {
            CmdDropItem();
            playerCamera.m_Lens.FieldOfView = baseFOV;
            ClearPrimarySlot();
        }
    }

    [ClientRpc]
    public void RpcTryDropItem()
    {
        TryDropItem();
    }

    [Command]
    private void CmdTryPickUpItem(NetworkIdentity itemIdentity)
    {
        H_WorldItem item = itemIdentity.GetComponent<H_WorldItem>();

        if (item != null)
        {
            item.PickUpItem(netIdentity);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdDropItem()
    {
        Vector3 position = dropPoint.position;
        Quaternion rotation = dropPoint.rotation;
        GameObject droppedObject = Instantiate(currentObject.worldDropItem, position, rotation);

        droppedObject.GetComponent<Rigidbody>().AddTorque(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
        droppedObject.GetComponent<Rigidbody>().AddForce(dropPoint.forward, ForceMode.Impulse);

        NetworkServer.Destroy(primaryClientObject.gameObject);
        NetworkServer.Destroy(primaryObserverObject.gameObject);

        RpcClearPrimarySlot();

        isHoldingItem = false;

        NetworkServer.Spawn(droppedObject);
    }

    [ClientRpc]
    public void RpcEquipPrimary(GameObject clientObject, GameObject observerObject)
    {
        primaryClientObject = clientObject;
        primaryObserverObject = observerObject;

        primaryClientObject.transform.parent = primaryEquipPointClient;
        primaryClientObject.transform.localPosition = Vector3.zero;
        primaryClientObject.transform.localRotation = Quaternion.identity;

        primaryObserverObject.transform.parent = primaryEquipPointObserver;
        primaryObserverObject.transform.localPosition = Vector3.zero;
        primaryObserverObject.transform.localRotation = Quaternion.identity;

        isHoldingItem = true;

        currentObject = primaryClientObject.GetComponent<H_ItemBase>();
        currentObject.Initialize();
    }

    [ClientRpc]
    public void RpcEquipSidearm(GameObject clientObject, GameObject observerObject)
    {
        sidearmClientObject = clientObject;
        sidearmObserverObject = observerObject;

        sidearmClientObject.transform.parent = sidearmEquipPointClient;
        sidearmClientObject.transform.localPosition = Vector3.zero;
        sidearmClientObject.transform.localRotation = Quaternion.identity;

        sidearmObserverObject.transform.parent = sidearmEquipPointObserver;
        sidearmObserverObject.transform.localPosition = Vector3.zero;
        sidearmObserverObject.transform.localRotation = Quaternion.identity;

        sidearmClientObject.GetComponent<H_ItemBase>().Initialize();
    }

    [ClientRpc]
    public void RpcEquipHolstered(GameObject clientObject, GameObject observerObject)
    {
        holsteredClientObject = clientObject;
        holsteredObserverObject = observerObject;

        holsteredClientObject.transform.parent = holsteredEquipPointClient;
        holsteredClientObject.transform.localPosition = Vector3.zero;
        holsteredClientObject.transform.localRotation = Quaternion.identity;

        holsteredObserverObject.transform.parent = holsteredEquipPointObserver;
        holsteredObserverObject.transform.localPosition = Vector3.zero;
        holsteredObserverObject.transform.localRotation = Quaternion.identity;

        holsteredClientObject.GetComponent<H_ItemBase>().Initialize();
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

    public void SetDead(bool value)
    {
        isDead = value;

        if (isDead)
        {
            CmdChangeSlot(0);
        }
    }

    public void ClearCurrentObject()
    {
        isHoldingItem = false;
        currentObject = null;
    }

    public void ClearPrimarySlot()
    {
        primaryItemIcon.sprite = null;
    }

    public void ClearSidearmSlot()
    {
        sidearmItemIcon.sprite = null;
    }

    public void ClearHolsteredSlot()
    {
        holsteredItemIcon.sprite = null;
    }

    [ClientRpc]
    public void RpcClearPrimarySlot()
    {
        ClearPrimarySlot();
    }

    [ClientRpc]
    public void RpcClearSidearmSlot()
    {
        ClearSidearmSlot();
    }

    [ClientRpc]
    public void RpcClearHolsteredSlot()
    {
        ClearHolsteredSlot();
    }

    public void SetAmmoUI(int ammoLoaded, int ammoPool, float reloadTime)
    {
        ammoLoadedText.text = ammoLoaded.ToString();
        ammoPoolText.text = ammoPool.ToString();

        reloadingImage.fillAmount = reloadTime;
    }

    public void ClearAmmoUI()
    {
        ammoLoadedText.text = "";
        ammoPoolText.text = "";

        reloadingImage.fillAmount = 0;
    }

}

public enum EquipmentSlot
{
    PrimaryItem,
    Sidearm,
    Holstered
}