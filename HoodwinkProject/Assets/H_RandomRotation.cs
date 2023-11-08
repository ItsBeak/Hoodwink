using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_RandomRotation : MonoBehaviour
{
    public Vector3 rotationBounds;

    void Start()
    {
        transform.Rotate(new Vector3(Random.Range(0, rotationBounds.x), Random.Range(0, rotationBounds.y), Random.Range(0, rotationBounds.z)));
    }
}
