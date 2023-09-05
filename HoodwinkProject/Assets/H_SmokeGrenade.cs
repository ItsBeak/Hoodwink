using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_SmokeGrenade : MonoBehaviour
{
    public float grenadeDelay = 3;
    float timer;
    bool isTriggered = false;

    public ParticleSystem smokeParticle;
    public AudioClip smokeAudioClip;
    AudioSource source;

    void Start()
    {
        timer = grenadeDelay;
        source = GetComponent<AudioSource>();
        source.PlayOneShot(smokeAudioClip);
    }

    void Update()
    {
        if (isTriggered)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            isTriggered = true;
            smokeParticle.Play();
        }

    }
}
