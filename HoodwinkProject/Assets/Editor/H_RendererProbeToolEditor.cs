using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(H_RendererProbeTool))]
public class H_RendererProbeToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        H_RendererProbeTool tool = (H_RendererProbeTool)target;

        if (GUILayout.Button("Scan for Child Renderers"))
        {
            tool.ScanProbes();
        }

        if (GUILayout.Button("Disable Probes on Child Renderers"))
        {
            tool.DisableProbes();
        }

        if (GUILayout.Button("Enable Probes on Child Renderers"))
        {
            tool.EnableProbes();
        }

    }
}
