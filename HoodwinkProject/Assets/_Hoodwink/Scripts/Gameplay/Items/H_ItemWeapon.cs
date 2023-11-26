using UnityEngine;
using Mirror;
using System;
using Random = UnityEngine.Random;
using Cinemachine.Utility;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class H_ItemWeapon : H_ItemBase
{
    [Header("Base Weapon Data")]
    public int damage;
    public float range;
    public bool sidearmMode;

    [Header("Aiming")]
    public Vector3 restPosition;
    public Vector3 aimPosition;
    public float aimSpeed;
    public float aimedFOV;
    [HideInInspector] public bool isAiming;

    [Header("Camera Recoil")]
    [SerializeField] private float cameraVerticalRecoilHipFire;
    [SerializeField] private float cameraHorizontalRecoilHipFire;
    [SerializeField] private float cameraRotationalRecoilHipFire;

    [SerializeField] private float cameraVerticalRecoilAimedFire;
    [SerializeField] private float cameraHorizontalRecoilAimedFire;
    [SerializeField] private float cameraRotationalRecoilAimedFire;

    [SerializeField] private float cameraRecoilForce;
    [SerializeField] private float cameraReturnSpeed;

    [Header("Weapon Recoil")]
    [SerializeField] private float weaponVerticalRecoilHipFire;
    [SerializeField] private float weaponHorizontalRecoilHipFire;
    [SerializeField] private float weaponRotationalRecoilHipFire;

    [SerializeField] private float weaponVerticalRecoilAimedFire;
    [SerializeField] private float weaponHorizontalRecoilAimedFire;
    [SerializeField] private float weaponRotationalRecoilAimedFire;

    [SerializeField] private float weaponRecoilForce;
    [SerializeField] private float weaponReturnSpeed;

    H_Recoil weaponRecoil;

    [Header("Effects")]
    public GameObject bulletHolePrefab;
    H_WeaponEffects clientEffects;
    H_WeaponEffects observerEffects;

    [Header("Animations")]
    public Animator viewmodelAnimator;
    public Renderer jacketRenderer;

    [Header("Bullet Settings")]
    public int bulletsPerShot = 1;
    public LayerMask shootableLayers;

    [Header("Spread Settings")]
    [Range(0f, 3f)] public float defaultBulletSpread;
    [Range(0f, 3f)] public float aimedBulletSpread;
    [Range(0f, 3f)] public float movingBulletSpread;
    public float perShotBloom;
    public float maxBloom;
    public float bloomRecoveryMultiplier = 1;

    public bool drawDebugSpreadRays;
    float bulletSpread;
    float bulletBloom = 0;

    [Header("Crosshair Settings")]
    public float crosshairOffset;
    public float crosshairMultiplier;
    public RectTransform crosshairPieceTop, crosshairPieceBottom, crosshairPieceLeft, crosshairPieceRight;
    public CanvasGroup crosshairFade;

    [Header("Ammo Settings")]
    public int maxAmmo;
    public int clipSize;
    public int startingAmmo;
    public float reloadTime;
    [HideInInspector] public int ammoLoaded;
    [HideInInspector] public int ammoPool;
    bool lockTrigger = false;

    [Header("Ammo UI")]
    public TextMeshProUGUI ammoFillText;
    public TextMeshProUGUI ammoShadowText;
    public GameObject silencerUIParent;
    public Image silencerIcon;

    [Header("Debugging")]
    public bool enableDebugLogs;

    public override void Initialize()
    {
        base.Initialize();

        clientEffects = GetComponent<H_WeaponEffects>();
        weaponRecoil = GetComponent<H_Recoil>();

        if (sidearmMode)
        {
            observerEffects = equipment.sidearmEquipPointObserver.GetComponentInChildren<H_WeaponEffects>();
        }
        else
        {
            observerEffects = equipment.primaryEquipPointObserver.GetComponentInChildren<H_WeaponEffects>();
        }

        ammoPool = startingAmmo;
        ammoLoaded = clipSize;

        bulletSpread = defaultBulletSpread;

        Debug.Log("Initializing sidearm");

        if (jacketRenderer)
        {
            //jacketRenderer.material.color = equipment.brain.agentData.primaryColour;
        }
    }

    public override void Update()
    {
        base.Update();

        if (!isOwned || !equipment)
            return;

        if (!waitForSecondaryKeyReleased)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, restPosition, aimSpeed * Time.deltaTime);
            equipment.playerCamera.m_Lens.FieldOfView = Mathf.Lerp(equipment.playerCamera.m_Lens.FieldOfView, equipment.baseFOV, aimSpeed * Time.deltaTime);
            isAiming = false;
        }

        if (equipment.CheckBusy())
            return;

        if (waitForSecondaryKeyReleased)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, aimPosition, aimSpeed * Time.deltaTime);
            equipment.playerCamera.m_Lens.FieldOfView = Mathf.Lerp(equipment.playerCamera.m_Lens.FieldOfView, aimedFOV, aimSpeed * Time.deltaTime);
            isAiming = true;
        }

        if (!equipment.isPrimaryUseKeyPressed)
        {
            lockTrigger = false;
        }

        if (equipment.controller.isMoving)
        {
            if (isAiming)
            {
                bulletSpread = Mathf.Lerp(bulletSpread, Mathf.Lerp(movingBulletSpread, aimedBulletSpread, 0.5f), Time.deltaTime * 4);
                crosshairFade.alpha = Mathf.Lerp(crosshairFade.alpha, 0.2f, Time.deltaTime * 6);
            }
            else
            {
                bulletSpread = Mathf.Lerp(bulletSpread, movingBulletSpread, Time.deltaTime * 4);
                crosshairFade.alpha = Mathf.Lerp(crosshairFade.alpha, 1f, Time.deltaTime * 6);
            }
        }
        else
        {
            if (isAiming)
            {
                bulletSpread = Mathf.Lerp(bulletSpread, aimedBulletSpread, Time.deltaTime * 3.5f);
                crosshairFade.alpha = Mathf.Lerp(crosshairFade.alpha, 0f, Time.deltaTime * 6);
            }
            else
            {
                bulletSpread = Mathf.Lerp(bulletSpread, defaultBulletSpread, Time.deltaTime * 3);
                crosshairFade.alpha = Mathf.Lerp(crosshairFade.alpha, 1f, Time.deltaTime * 6);
            }
        }

        bulletBloom = Mathf.Lerp(bulletBloom, 0, Time.deltaTime * bloomRecoveryMultiplier);

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (equipment.brain.currentAlignment == AgentAlignment.Spy)
            {
                StartCoroutine(SilencerChange());
            }
        }

        float spread = crosshairOffset + ((bulletSpread + bulletBloom) * crosshairMultiplier);

        Vector3 spreadVector = new Vector3(0, spread, 0);

        crosshairPieceTop.localPosition = spreadVector;
        crosshairPieceBottom.localPosition = spreadVector;
        crosshairPieceLeft.localPosition = spreadVector;
        crosshairPieceRight.localPosition = spreadVector;

        silencerUIParent.SetActive(equipment.brain.currentAlignment == AgentAlignment.Spy ? true : false);

        ammoFillText.text = ammoLoaded + "/" + ammoPool;
        ammoShadowText.text = ammoLoaded + "/" + ammoPool;

        if (viewmodelAnimator)
            viewmodelAnimator.SetBool("isAiming", isAiming);
    }

    public override void PrimaryUse()
    {
        if (lockTrigger)
            return;
        if (enableDebugLogs)
            Debug.Log("Shooting");

        if (ammoLoaded > 0)
        {

            ammoLoaded--;

            for (int i = 0; i < bulletsPerShot; i++)
            {
                RaycastHit hit;

                Vector3 bulletDirection = new Vector3(0, 0, 10);

                bulletDirection.x += Random.Range(-bulletSpread + -bulletBloom, bulletSpread + bulletBloom);
                bulletDirection.y += Random.Range(-bulletSpread + -bulletBloom, bulletSpread + bulletBloom);

                bulletDirection = equipment.playerCamera.transform.localToWorldMatrix * bulletDirection;

                if (Physics.Raycast(equipment.playerCamera.transform.position, bulletDirection, out hit, range, shootableLayers))
                {

                    if (!hit.collider.transform.IsChildOf(equipment.transform))
                    {
                        if (enableDebugLogs)
                            Debug.LogWarning("Hit Object: " + hit.collider.name);

                        var hitbox = hit.collider.gameObject.GetComponent<H_PlayerHitbox>();

                        if (hitbox)
                        {
                            hitbox.OnHit(damage);
                            equipment.SpawnHitMarker();
                        }
                        else
                        {
                            CmdSpawnImpact(hit.point, hit.normal);
                        }
                    }
                    else
                    {
                        if (enableDebugLogs)
                            Debug.LogError("Bullet hit object: " + hit.collider.gameObject.name + ", which is a child of this player. Check layermask settings on both the hit object and the gun firing this bullet");
                    }
                }
            }

            bulletBloom += perShotBloom;
            Mathf.Clamp(bulletBloom, 0, maxBloom);
            crosshairFade.alpha += 0.2f;

            if (isAiming)
            {
                equipment.cameraRecoil.AddRecoil(cameraVerticalRecoilAimedFire, cameraHorizontalRecoilAimedFire, cameraRotationalRecoilAimedFire, cameraRecoilForce, cameraReturnSpeed);
                weaponRecoil.AddRecoil(weaponVerticalRecoilAimedFire, weaponHorizontalRecoilAimedFire, weaponRotationalRecoilAimedFire, weaponRecoilForce, weaponReturnSpeed);
            }
            else
            {
                equipment.cameraRecoil.AddRecoil(cameraVerticalRecoilHipFire, cameraHorizontalRecoilHipFire, cameraRotationalRecoilHipFire, cameraRecoilForce, cameraReturnSpeed);
                weaponRecoil.AddRecoil(weaponVerticalRecoilHipFire, weaponHorizontalRecoilHipFire, weaponRotationalRecoilHipFire, weaponRecoilForce, weaponReturnSpeed);
            }

            if (clientEffects)
            {
                clientEffects.PlayFireLocal();
                observerEffects.CmdPlayFire();
            }


            base.PrimaryUse();

        }
        else
        {
            clientEffects.PlayDryFireLocal();
            observerEffects.CmdPlayDryFire();

            lockTrigger = true;
        }
    }

    public override void SecondaryUse()
    {
        base.SecondaryUse();
    }

    public override void AlternateUse()
    {
        if (ammoLoaded == clipSize || ammoPool == 0)
        {
            return;
        }

        base.AlternateUse();

        StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        equipment.SetBusy(true);

        if (viewmodelAnimator)
            viewmodelAnimator.SetTrigger("Reload");

        yield return new WaitForSeconds(0.25f);

        while (ammoLoaded < clipSize && ammoPool > 0)
        {
            ammoLoaded++;
            ammoPool--;
        }

        clientEffects.PlayReloadLocal();
        observerEffects.CmdPlayReload();

        yield return new WaitForSeconds(reloadTime);

        equipment.SetBusy(false);
    }

    public void ToggleSilencer()
    {
        clientEffects.ToggleSilencer();
        observerEffects.ToggleSilencer();
    }

    [Command]
    void CmdSpawnImpact(Vector3 position, Vector3 normal)
    {
        RpcSpawnImpact(position, normal);
    }

    [ClientRpc]
    void RpcSpawnImpact(Vector3 position, Vector3 normal)
    {
        GameObject newBulletHole = GameObject.Instantiate(bulletHolePrefab, position, Quaternion.identity);
        newBulletHole.transform.LookAt(position + normal);
    }

    private void OnDrawGizmos()
    {   
        if (drawDebugSpreadRays)
        {
            Gizmos.color = Color.red;

            Vector3 rayDir = new Vector3(0, 0, 10);
            rayDir.x = bulletSpread + bulletBloom;
            rayDir = equipment.playerCamera.transform.localToWorldMatrix * rayDir;
            rayDir += equipment.playerCamera.transform.position;
            Gizmos.DrawLine(equipment.playerCamera.transform.position, rayDir);

            rayDir = new Vector3(0, 0, 10);
            rayDir.x = -bulletSpread + -bulletBloom;
            rayDir = equipment.playerCamera.transform.localToWorldMatrix * rayDir;
            rayDir += equipment.playerCamera.transform.position;
            Gizmos.DrawLine(equipment.playerCamera.transform.position, rayDir);

            rayDir = new Vector3(0, 0, 10);
            rayDir.y = bulletSpread + bulletBloom;
            rayDir = equipment.playerCamera.transform.localToWorldMatrix * rayDir;
            rayDir += equipment.playerCamera.transform.position;
            Gizmos.DrawLine(equipment.playerCamera.transform.position, rayDir);

            rayDir = new Vector3(0, 0, 10);
            rayDir.y = -bulletSpread + -bulletBloom;
            rayDir = equipment.playerCamera.transform.localToWorldMatrix * rayDir;
            rayDir += equipment.playerCamera.transform.position;
            Gizmos.DrawLine(equipment.playerCamera.transform.position, rayDir);
        }
    }

    IEnumerator SilencerChange()
    {
        equipment.SetBusy(true);

        if (!clientEffects.isSilenced)
        {
            clientEffects.ToggleDecorativeSilencer(true);

            viewmodelAnimator.SetTrigger("SilencerAdd");

            yield return new WaitForSeconds(1.5f);

            ToggleSilencer();

            yield return new WaitForSeconds(4f);

            equipment.SetBusy(false);

            clientEffects.ToggleDecorativeSilencer(false);
        }
        else
        {
            clientEffects.ToggleDecorativeSilencer(true);

            viewmodelAnimator.SetTrigger("SilencerRemove");

            yield return new WaitForSeconds(0.5f);

            ToggleSilencer();

            yield return new WaitForSeconds(3f);

            equipment.SetBusy(false);

            clientEffects.ToggleDecorativeSilencer(false);
        }
    }

    public void ClearAmmoUI()
    {
        ammoFillText.text = "";
        ammoShadowText.text = "";
    }

}
