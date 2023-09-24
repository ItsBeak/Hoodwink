using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_ObjectManager : NetworkBehaviour
{
    H_LevelData currentLevel;

    [Header("Round Settings & Loot")]
    public GameObject[] items;

    public GameObject faxMachinePrefab;
    public GameObject shredderPrefab;
    public GameObject computerPrefab;
    public GameObject keyMachinePrefab;

    [Header("Level Cleanup Settings")]
    public LayerMask cleanupLayers;

    [Header("Debugging")]
    public bool enableDebugLogs;

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

        int randomShredderSet = Random.Range(0, currentLevel.documentObjectives.Length);

        GameObject shredder = Instantiate(shredderPrefab, currentLevel.documentObjectives[randomShredderSet].shredderLocation.position, currentLevel.documentObjectives[randomShredderSet].shredderLocation.rotation);
        NetworkServer.Spawn(shredder);

        GameObject faxMachine = Instantiate(faxMachinePrefab, currentLevel.documentObjectives[randomShredderSet].faxLocation.position, currentLevel.documentObjectives[randomShredderSet].faxLocation.rotation);
        NetworkServer.Spawn(faxMachine);

        int randomComputerSet = Random.Range(0, currentLevel.computerObjectives.Length);

        GameObject computer = Instantiate(computerPrefab, currentLevel.computerObjectives[randomComputerSet].computerLocation.position, currentLevel.computerObjectives[randomComputerSet].computerLocation.rotation);
        NetworkServer.Spawn(computer);

        GameObject keyMachine = Instantiate(keyMachinePrefab, currentLevel.computerObjectives[randomComputerSet].keyMachineLocation.position, currentLevel.computerObjectives[randomComputerSet].keyMachineLocation.rotation);
        NetworkServer.Spawn(keyMachine);
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
