using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class H_MeleeEffects : NetworkBehaviour
{
    [Header("Audio")]
    AudioSource source;
    public AudioClip[] swingClips;
    public AudioClip[] hitClips;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlaySwingLocal()
    {
        source.PlayOneShot(swingClips[Random.Range(0, swingClips.Length)]);
    }

    [Command(requiresAuthority = false)]
    public void CmdPlaySwing()
    {
        RpcPlaySwing();
    }

    [ClientRpc(includeOwner = false)]
    void RpcPlaySwing()
    {
        source.PlayOneShot(swingClips[Random.Range(0, swingClips.Length)]);
    }

    public void PlayHitLocal()
    {
        source.PlayOneShot(hitClips[Random.Range(0, hitClips.Length)]);
    }

    [Command(requiresAuthority = false)]
    public void CmdPlayHit()
    {
        RpcPlayHit();
    }

    [ClientRpc(includeOwner = false)]
    void RpcPlayHit()
    {
        source.PlayOneShot(hitClips[Random.Range(0, hitClips.Length)]);
    }

}
