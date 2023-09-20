using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class H_TeamMemberDetails : MonoBehaviour
{
    public string memberName;
    public string memberDescription;

    H_TeamMemberDisplay display;

    bool isHovered;
    public HDAdditionalLightData spotlight;

    public float focusedBrightness, unfocusedBrightness;

    public AudioClip hoverClip;
    AudioSource source;
    [Range(0, 2)] public float pitch;

    void Start()
    {
        display = FindObjectOfType(typeof(H_TeamMemberDisplay)).GetComponent<H_TeamMemberDisplay>();
        source = GetComponent<AudioSource>();
        source.pitch = pitch;
    }

    private void OnMouseEnter()
    {
        if (display)
        {
            display.memberName.text = memberName;
            display.memberDescription.text = memberDescription;
            isHovered = true;

            source.PlayOneShot(hoverClip);
        }
    }

    private void OnMouseExit()
    {
        if (display)
        {
            display.memberName.text = "";
            display.memberDescription.text = "";
            isHovered = false;
        }
    }

    private void Update()
    {
        spotlight.intensity = Mathf.Lerp(spotlight.intensity, isHovered ? focusedBrightness : unfocusedBrightness, Time.deltaTime * 2);
    }
}
