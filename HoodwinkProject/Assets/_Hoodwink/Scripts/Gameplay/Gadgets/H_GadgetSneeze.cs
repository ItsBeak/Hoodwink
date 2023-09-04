using UnityEngine;
using UnityEngine.VFX;
using Mirror;

public class H_GadgetSneeze : H_GadgetBase
{
    AudioSource source;

    [Header("Sneeze Settings")]
    public AudioClip[] sneezeClips;
    public VisualEffect sneezeFX;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    [ClientRpc]
    public override void RpcUseGadgetPrimary()
    {
        base.RpcUseGadgetPrimary();

        source.PlayOneShot(sneezeClips[Random.Range(0, sneezeClips.Length)]);
        sneezeFX.Play();
    }
}
