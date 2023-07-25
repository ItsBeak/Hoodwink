using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_ObjectManager : NetworkBehaviour
{

    PlayerSettings settings;
    H_GameManager manager;
    H_LevelData currentLevel;

    [Header("Round Settings & Loot")]
    public GameObject[] items;

    [Header("Level Cleanup Settings")]
    public LayerMask cleanupLayers;

    [Header("Debugging")]
    public bool enableDebugLogs;

    private void Start()
    {
        manager = GetComponent<H_GameManager>();
    }
    public void SetPlayerSettings(PlayerSettings newSettings)
    {
        settings = newSettings;
    }

    [Server]
    public void SpawnObjects(PlayerSettings newSettings)
    {
        currentLevel = FindObjectOfType<H_LevelData>();

        List<Transform> itemSpawns = new List<Transform>();

        foreach (Transform point in currentLevel.itemSpawnPoints)
        {
            itemSpawns.Add(point);
        }

        for (int i = 0; i < newSettings.itemsToSpawn; i++)
        {
            int spawnPoint = Random.Range(0, itemSpawns.Count);

            GameObject newItem = Instantiate(items[Random.Range(0, items.Length)], itemSpawns[spawnPoint].position, itemSpawns[spawnPoint].rotation);
            NetworkServer.Spawn(newItem);
            itemSpawns.Remove(itemSpawns[spawnPoint]);

            if (enableDebugLogs)
                Debug.Log("Spawning new item: " + newItem.name);
        }

    }

    [Server]
    public void CleanupObjects()
    {
        foreach (Collider ob in Physics.OverlapBox(currentLevel.cleanupCentre, currentLevel.cleanupBounds, Quaternion.identity, cleanupLayers))
        {
            if (ob)
            {
                if (ob.GetComponent<NetworkIdentity>())
                {
                    if (enableDebugLogs)
                        Debug.Log("Removing item: " + ob.name);

                    NetworkServer.Destroy(ob.gameObject);
                }
            }
        }
    }

}
