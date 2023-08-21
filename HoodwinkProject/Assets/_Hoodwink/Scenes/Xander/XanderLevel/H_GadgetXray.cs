using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_GadgetXray : H_GadgetBase
{
    [Header("X-Ray Settings")]
    [SerializeField] GameObject _camera;
    [SerializeField] Animator _xray;

    bool active;

    [Header("Audio Settings")]
    public AudioClip[] XrayUpClips;
    public AudioClip[] XrayDownClips;
    public AudioSource source;


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
        _camera.gameObject.SetActive(true);
    }
    void XrayOff()
    {
        _camera.gameObject.SetActive(false);
    }

}
