using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class H_GadgetXray : H_GadgetBase
{
    [Header("X-Ray Settings")]
    [SerializeField] GameObject _effect;
    [SerializeField] Animator _xray;


    bool active;

    [Header("Audio Settings")]
    public AudioClip[] XrayUpClips;
    public AudioClip[] XrayDownClips;
    public AudioSource source;

    private void Start()
    {
        //Get the camera and turn it off, had a mind fry and dont remember why i did this
        Camera parent = GetComponentInChildren<Camera>();
        _effect.gameObject.SetActive(false);
        active = true;
    }
    public override void UseGadget()
    {
        //trigger the equip animation
        if (active)
        {
            _xray.SetTrigger("Equip");
        }
        //Trigger the remove animation
        if (!active)
        {
            _xray.SetTrigger("Remove");
        }
    }

    //Both activated in events within the animations
    void XrayOn()
    {
        _effect.gameObject.SetActive(true);
        active = false;
    }
    void XrayOff()
    {
        _effect.gameObject.SetActive(false);
        active = true;
    }

}
