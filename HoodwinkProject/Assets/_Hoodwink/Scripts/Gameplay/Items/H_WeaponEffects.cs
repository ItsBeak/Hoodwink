using UnityEngine;
using Mirror;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.VFX;

public class H_WeaponEffects : NetworkBehaviour
{
    [Header("Audio")]
    AudioSource source;
    public AudioClip[] shootClips;
    public AudioClip[] reloadClip;
    public AudioClip[] dryFireClips;

    [Header("Slide")]
    public Transform slide;
    public Vector3 slideTargetPosition;
    public float slideSpeed = 1f;
    Vector3 slideRestPosition;

    [Header("Muzzle Flash")]
    public HDAdditionalLightData flashLight;
    public VisualEffect flashEffect;
    public float flashBrightness;
    public float flashSpeed;

    [Header("Casing")]
    public ParticleSystem casing;


    void Start()
    {
        source = GetComponent<AudioSource>();

        if (slide)
            slideRestPosition = slide.localPosition;
    }

    void Update()
    {
        if (slide)
        {
            slide.localPosition = Vector3.Lerp(slide.localPosition, slideRestPosition, slideSpeed * Time.deltaTime);
        }

        if (flashLight.intensity > 0)
        {
            flashLight.intensity -= flashSpeed * Time.deltaTime;
        }
    }

    public void TriggerSlide()
    {
        if (slide)
            slide.localPosition = slideTargetPosition;
    }

    public void TriggerFlash()
    {
        flashLight.intensity = flashBrightness;

        if (flashEffect)
            flashEffect.Play();
    }

    public void PlayFireLocal()
    {
        source.PlayOneShot(shootClips[Random.Range(0, shootClips.Length)]);
        TriggerFlash();
        TriggerSlide();

        if (casing)
            casing.Play();
    }

    [Command(requiresAuthority = false)]
    public void CmdPlayFire()
    {
        RpcPlayFire();
    }

    [ClientRpc(includeOwner = false)]
    void RpcPlayFire()
    {

        source.PlayOneShot(shootClips[Random.Range(0, shootClips.Length)]);
        TriggerFlash();
        TriggerSlide();

        if (casing)
            casing.Play();
    }

    public void PlayDryFireLocal()
    {
        source.PlayOneShot(dryFireClips[Random.Range(0, dryFireClips.Length)]);
    }

    [Command(requiresAuthority = false)]
    public void CmdPlayDryFire()
    {
        RpcPlayDryFire();
    }

    [ClientRpc(includeOwner = false)]
    void RpcPlayDryFire()
    {
        source.PlayOneShot(dryFireClips[Random.Range(0, dryFireClips.Length)]);
    }

    public void PlayReloadLocal()
    {
        source.PlayOneShot(reloadClip[Random.Range(0, reloadClip.Length)]);
    }

    [Command(requiresAuthority = false)]
    public void CmdPlayReload()
    {
        RpcPlayReload();
    }

    [ClientRpc(includeOwner = false)]
    void RpcPlayReload()
    {
        source.PlayOneShot(reloadClip[Random.Range(0, reloadClip.Length)]);
    }
}
