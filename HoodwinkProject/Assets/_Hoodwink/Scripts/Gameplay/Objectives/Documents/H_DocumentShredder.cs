using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using static UnityEngine.ParticleSystem;

public class H_DocumentShredder : NetworkBehaviour
{

    public int scoreChange;
    [Range(0, 2)]public float scoreChangeInterval;

    [Header("Effects")]
    public ParticleSystem particles;

    [Header("Audio")]
    public AudioClip useClip;
    public AudioClip stopUseClip;

    AudioSource source;

    [Header("SyncVars")]
    [SyncVar(hook = nameof(OnDocumentChanged))] public bool containsDocument;

    [Header("Debugging")]
    public bool enableDebugLogs;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    [Command(requiresAuthority = false)]
    public void CmdAddDocument()
    {
        StartCoroutine(ActivateShredder());
    }

    void OnDocumentChanged(bool oldState, bool newState)
    {
        if (newState)
        {
            source.PlayOneShot(useClip);
            particles.Play();
        }
        else
        {
            source.PlayOneShot(stopUseClip);
            particles.Stop();
        }
    }

    IEnumerator ActivateShredder()
    {
        containsDocument = true;

        int scoreLeft = scoreChange;

        if (enableDebugLogs)
            Debug.Log("Shredder started");

        while (scoreLeft > 0)
        {
            yield return new WaitForSeconds(scoreChangeInterval);

            H_GameManager.instance.CmdUpdateEvidence(-1);
            scoreLeft--;

            if (enableDebugLogs)
                Debug.Log("Removed evidence");
        }

        if (enableDebugLogs)
            Debug.Log("Shredder completed");

        containsDocument = false;

        yield return null;
    }
}
