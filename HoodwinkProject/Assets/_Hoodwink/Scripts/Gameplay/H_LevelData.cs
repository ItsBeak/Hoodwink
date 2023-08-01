using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_LevelData : MonoBehaviour
{
    public Transform[] playerSpawnPoints;

    public Transform[] itemSpawnPoints;

    [Header("Level Cleanup Settings")]
    public Vector3 cleanupCentre;
    public Vector3 cleanupBounds;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(cleanupCentre, cleanupBounds);
    }
}
