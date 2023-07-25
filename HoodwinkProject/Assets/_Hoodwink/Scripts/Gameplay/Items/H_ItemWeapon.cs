using UnityEngine;
using Mirror;
using System;
using Random = UnityEngine.Random;

public class H_ItemWeapon : H_ItemBase
{
    [Header("Base Weapon Data")]
    public int damage;
    public float range;
    public bool sidearmMode;

    //[Header("Aiming")]
    //public Vector3 restPosition;
    //public Vector3 aimPosition;
    //public float aimSpeed;
    //public float aimedFOV;
    //[HideInInspector] public bool isAiming;

    [Header("Effects")]
    public GameObject bulletHolePrefab;
    H_WeaponEffects clientEffects;
    H_WeaponEffects observerEffects;

    [Header("Bullet Settings")]
    [Range(0f, 1f)]
    public float bulletSpread;
    public int bulletsPerShot = 1;
    public LayerMask shootableLayers;

    [Header("Ammo Settings")]
    public int maxAmmo;
    public int clipSize;
    public int startingAmmo;
    [HideInInspector] public int ammoLoaded;
    [HideInInspector] public int ammoPool;
    bool lockTrigger = false;

    //[Header("Animation")]
    //public Animator anim;

    [Header("Debugging")]
    public bool enableDebugLogs;

    public override void Initialize()
    {
        base.Initialize();

        clientEffects = GetComponent<H_WeaponEffects>();

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

        Debug.Log("Initializing sidearm");
    }

    public override void Update()
    {
        base.Update();

        if (!isOwned || !equipment)
            return;

        //if (waitForSecondaryKeyReleased)
        //{
        //    transform.localPosition = Vector3.Lerp(transform.localPosition, aimPosition, aimSpeed * Time.deltaTime);
        //    equipment.playerCamera.m_Lens.FieldOfView = Mathf.Lerp(equipment.playerCamera.m_Lens.FieldOfView, aimedFOV, aimSpeed * Time.deltaTime);
        //    isAiming = true;
        //}
        //else
        //{
        //    transform.localPosition = Vector3.Lerp(transform.localPosition, restPosition, aimSpeed * Time.deltaTime);
        //    equipment.playerCamera.m_Lens.FieldOfView = Mathf.Lerp(equipment.playerCamera.m_Lens.FieldOfView, equipment.baseFOV, aimSpeed * Time.deltaTime);
        //    isAiming = false;
        //}

        if (!equipment.isPrimaryUseKeyPressed)
        {
            lockTrigger = false;
        }

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

                Vector3 bulletDirection = new Vector3(0, 0, 1);

                bulletDirection.x += Random.Range(-bulletSpread * 0.1f, bulletSpread * 0.1f);
                bulletDirection.y += Random.Range(-bulletSpread * 0.1f, bulletSpread * 0.1f);

                bulletDirection = equipment.playerCamera.transform.localToWorldMatrix * bulletDirection;

                if (Physics.Raycast(equipment.playerCamera.transform.position, bulletDirection, out hit, range, shootableLayers))
                {

                    if (!hit.collider.transform.IsChildOf(equipment.transform))
                    {
                        if (enableDebugLogs)
                            Debug.LogWarning("Hit Object: " + hit.collider.name);

                        var health = hit.collider.gameObject.GetComponentInParent<H_PlayerHealth>();

                        if (health)
                        {
                            health.Damage(damage);
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

            //if (isAiming)
            //{
            //    equipment.cameraRecoil.AddRecoil(cameraVerticalRecoilAimedFire, cameraHorizontalRecoilAimedFire, cameraRotationalRecoilAimedFire);
            //    equipment.weaponRecoil.AddRecoil(weaponVerticalRecoilAimedFire, weaponHorizontalRecoilAimedFire, weaponRotationalRecoilAimedFire);
            //}
            //else
            //{
            //    equipment.cameraRecoil.AddRecoil(cameraVerticalRecoilHipFire, cameraHorizontalRecoilHipFire, cameraRotationalRecoilHipFire);
            //    equipment.weaponRecoil.AddRecoil(weaponVerticalRecoilAimedFire, weaponHorizontalRecoilAimedFire, weaponRotationalRecoilAimedFire);
            //}

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

        LoadAmmo();
    }

    [ClientRpc]
    public override void RpcPrimaryUse()
    {
        //playerEffects.CmdPlayFire();
    }

    [ClientRpc]
    public override void RpcSecondaryUse()
    {

    }

    [ClientRpc]
    public override void RpcAlternateUse()
    {
        //need to enable animator while busy, play reload, then disable to allow for procedurals to continue
        //anim.SetTrigger("Reload");

        //playerEffects.CmdPlayReload();

    }

    void LoadAmmo()
    {
        while (ammoLoaded < clipSize && ammoPool > 0)
        {
            ammoLoaded++;
            ammoPool--;
        }
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

}
