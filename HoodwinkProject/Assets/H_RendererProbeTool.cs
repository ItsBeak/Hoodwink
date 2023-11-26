using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class H_RendererProbeTool : MonoBehaviour
{
    public void ScanProbes()
    {
        int number = 0;

        foreach (var renderer in transform.GetComponentsInChildren<Renderer>())
        {
            number++;
        }

        Debug.Log(number + " renderer components found");

    }

    public void DisableProbes()
    {
        int number = 0;

        foreach (var renderer in transform.GetComponentsInChildren<Renderer>())
        {
            number++;

            renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        }

        Debug.Log(number + " renderers light probe usage disabled");

    }

    public void EnableProbes()
    {
        int number = 0;

        foreach (var renderer in transform.GetComponentsInChildren<Renderer>())
        {
            number++;

            renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;
        }

        Debug.Log(number + " renderers light probe usage enabled");
    }
}

