using UnityEngine;
using Mirror;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.VFX;

public class H_WeaponEffects : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnSilencedChanged))]
    public bool isSilenced;

    [Header("Audio")]
    public AudioSource source;
    public AudioClip[] shootClips;
    public AudioClip[] shootSilencedClips;

    public AudioClip[] reloadClip;
    public AudioClip[] dryFireClips;

    [Header("Slide")]
    public Transform slide;
    public Vector3 slideTargetPosition;
    public float slideSpeed = 1f;
    Vector3 slideRestPosition;

    [Header("Muzzle Flash - Unsuppressed")]
    public HDAdditionalLightData unsuppressedFlashLight;
    public ParticleSystem unsuppressedFlash;
    public float unsuppressedFlashBrightness;
    public float unsuppressedFlashSpeed;
    public float unsuppressedAudioDistance;

    [Header("Muzzle Flash - Suppressed")]
    public HDAdditionalLightData suppressedFlashLight;
    public ParticleSystem suppressedFlash;
    public float suppressedFlashBrightness;
    public float suppressedFlashSpeed;
    public float suppressedAudioDistance;


    [Header("Casing")]
    public ParticleSystem casing;

    [Header("Silencer")]
    public GameObject silencer;


    void Start()
    {
        if (slide)
            slideRestPosition = slide.localPosition;

        silencer.SetActive(false);
    }

    void Update()
    {
        if (slide)
        {
            slide.localPosition = Vector3.Lerp(slide.localPosition, slideRestPosition, slideSpeed * Time.deltaTime);
        }

        if (unsuppressedFlashLight.intensity > 0)
        {
            unsuppressedFlashLight.intensity -= unsuppressedFlashSpeed * Time.deltaTime;
        }

        if (suppressedFlashLight.intensity > 0)
        {
            suppressedFlashLight.intensity -= suppressedFlashSpeed * Time.deltaTime;
        }
    }

    [Command]
    public void ToggleSilencer()
    {
        isSilenced = !isSilenced;
    }

    void OnSilencedChanged(bool oldSilenced, bool newSilenced)
    {
        silencer.SetActive(newSilenced);

        if (newSilenced)
        {
            source.maxDistance = suppressedAudioDistance;
        }
        else
        {
            source.maxDistance = unsuppressedAudioDistance;
        }
    }

    public void TriggerSlide()
    {
        if (slide)
            slide.localPosition = slideTargetPosition;
    }

    public void TriggerSuppressedFlash()
    {
        suppressedFlashLight.intensity = suppressedFlashBrightness;

        if (suppressedFlash)
            suppressedFlash.Play();
    }

    public void TriggerUnsuppressedFlash()
    {
        unsuppressedFlashLight.intensity = unsuppressedFlashBrightness;

        if (unsuppressedFlash)
            unsuppressedFlash.Play();
    }

    public void PlayFireLocal()
    {
        if (isSilenced)
        {
            source.PlayOneShot(shootSilencedClips[Random.Range(0, shootSilencedClips.Length)]);
            TriggerSuppressedFlash();
        }
        else
        {
            source.PlayOneShot(shootClips[Random.Range(0, shootClips.Length)]);
            TriggerUnsuppressedFlash();
        }

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
        if (isSilenced)
        {
            source.PlayOneShot(shootSilencedClips[Random.Range(0, shootSilencedClips.Length)]);
            TriggerSuppressedFlash();
        }
        else
        {
            source.PlayOneShot(shootClips[Random.Range(0, shootClips.Length)]);
            TriggerUnsuppressedFlash();
        }

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
