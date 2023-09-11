using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_PlayerHitbox : MonoBehaviour
{
    public enum HitboxType
    {
        Normal,
        Limb,
        Head
    }

    [HideInInspector] public HitboxType hitboxType;

    H_PlayerHealth health;

    public void OnHit(int damage)
    {
        switch (hitboxType)
        {
            case HitboxType.Normal: health.Damage(damage);
                break;
            case HitboxType.Limb: health.Damage(damage / 2);
                break;
            case HitboxType.Head: health.Damage(damage * 2);
                break;
        }

        Debug.Log("Hit hitbox of type: " + hitboxType.ToString());
    }

    public void Setup(H_PlayerHealth h)
    {
        health = h;
    }

}
