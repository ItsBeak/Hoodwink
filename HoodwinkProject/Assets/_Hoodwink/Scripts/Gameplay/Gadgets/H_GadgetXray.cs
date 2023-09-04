using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class H_GadgetXray : H_GadgetBase
{
    [Header("X-Ray Settings")]
    [SerializeField] GameObject cameraEffect;
    [SerializeField] Animator gadgetAnimator;

    bool isActive;

    [Header("Audio Settings")]
    public AudioClip[] XrayUpClips;
    public AudioClip[] XrayDownClips;
    public AudioSource source;

    private void Start()
    {
        cameraEffect.gameObject.SetActive(false);
        isActive = false;
    }
    public override void UseGadgetPrimary()
    {
        if (!isActive)
        {
            gadgetAnimator.SetTrigger("Equip");
        }
        else
        {
            gadgetAnimator.SetTrigger("Remove");
        }
    }

    public override void UseGadgetSecondary()
    {

    }

    void XrayOn()
    {
        cameraEffect.gameObject.SetActive(true);
        isActive = false;
    }
    void XrayOff()
    {
        cameraEffect.gameObject.SetActive(false);
        isActive = true;
    }

}
