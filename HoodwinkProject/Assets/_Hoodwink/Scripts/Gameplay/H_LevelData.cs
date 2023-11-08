using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_LevelData : MonoBehaviour
{
    public Transform[] playerSpawnPoints;

    public Transform[] itemSpawnPoints;
    public Transform[] phoneSpawnPoints;

    public DocumentObjectives[] documentObjectives;
    //public ComputerObjectives[] computerObjectives;

    [Header("Level Cleanup Settings")]
    public Vector3 cleanupCentre;
    public Vector3 cleanupBounds;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(cleanupCentre, cleanupBounds);
    }
}

[Serializable]
public struct DocumentObjectives
{
    public Transform[] shredderLocations;
    public Transform[] faxLocations;
}

[Serializable]
public struct ComputerObjectives
{
    public Transform computerLocation;
    public Transform keyMachineLocation;
}