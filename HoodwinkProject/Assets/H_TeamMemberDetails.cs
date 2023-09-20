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

    void Start()
    {
        display = FindObjectOfType(typeof(H_TeamMemberDisplay)).GetComponent<H_TeamMemberDisplay>();
    }

    private void OnMouseEnter()
    {
        Debug.Log("Started hovering over " + name);

        if (display)
        {
            display.memberName.text = memberName;
            display.memberDescription.text = memberDescription;
            isHovered = true;
        }
    }

    private void OnMouseExit()
    {
        Debug.Log("Stopped overing over " + name);

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
