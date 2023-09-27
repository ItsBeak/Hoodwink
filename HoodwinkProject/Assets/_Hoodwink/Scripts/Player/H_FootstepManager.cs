using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_FootstepManager : MonoBehaviour
{
    [Range(0f, 1f)]
    public float blendWeight;

    public AudioClip[] walkClips;
    public AudioClip[] runClips;
    public AudioClip[] crouchClips;

    AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    void WalkStep(AnimationEvent evt)
    {
        if (evt.animatorClipInfo.weight > blendWeight)
        {
            source.PlayOneShot(walkClips[Random.Range(0, walkClips.Length)]);
        }
    }

    void RunStep(AnimationEvent evt)
    {
        if (evt.animatorClipInfo.weight > blendWeight)
        {
            source.PlayOneShot(runClips[Random.Range(0, runClips.Length)]);
        }
    }

    void CrouchStep(AnimationEvent evt)
    {
        if (evt.animatorClipInfo.weight > blendWeight)
        {
            source.PlayOneShot(crouchClips[Random.Range(0, crouchClips.Length)]);
        }
    }
}
