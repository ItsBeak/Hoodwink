using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_Gizmo : MonoBehaviour
{
    public Vector3 bounds = new Vector3(1, 1, 1);
    public Vector3 offset = new Vector3(0, 0.5f, 0);

    public Color gizmoColour = new Color(255, 255, 255, 130);

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColour;
        Gizmos.DrawCube(transform.position + offset, bounds);
    }

}
