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
    //public GameObject computerPrefab;
    //public GameObject keyMachinePrefab;
    public GameObject phonePrefab;

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

        foreach (var shredderLocation in currentLevel.documentObjectives[randomShredderSet].shredderLocations)
        {
            GameObject shredder = Instantiate(shredderPrefab, shredderLocation.position, shredderLocation.rotation);
            NetworkServer.Spawn(shredder);
        }

        foreach (var faxLocation in currentLevel.documentObjectives[randomShredderSet].faxLocations)
        {
            GameObject faxMachine = Instantiate(faxMachinePrefab, faxLocation.position, faxLocation.rotation);
            NetworkServer.Spawn(faxMachine);
        }

        //int randomComputerSet = Random.Range(0, currentLevel.computerObjectives.Length);
        //
        //GameObject computer = Instantiate(computerPrefab, currentLevel.computerObjectives[randomComputerSet].computerLocation.position, currentLevel.computerObjectives[randomComputerSet].computerLocation.rotation);
        //NetworkServer.Spawn(computer);
        //
        //GameObject keyMachine = Instantiate(keyMachinePrefab, currentLevel.computerObjectives[randomComputerSet].keyMachineLocation.position, currentLevel.computerObjectives[randomComputerSet].keyMachineLocation.rotation);
        //NetworkServer.Spawn(keyMachine);

        List<Transform> phoneSpawns = new List<Transform>();

        foreach (Transform point in currentLevel.phoneSpawnPoints)
        {
            phoneSpawns.Add(point);
        }

        for (int i = 0; i < newSettings.phonesToSpawn; i++)
        {
            int spawnPoint = Random.Range(0, phoneSpawns.Count);

            GameObject newItem = Instantiate(phonePrefab, phoneSpawns[spawnPoint].position, phoneSpawns[spawnPoint].rotation);
            NetworkServer.Spawn(newItem);
            phoneSpawns.Remove(phoneSpawns[spawnPoint]);

            if (enableDebugLogs)
                Debug.Log("Spawning phone");
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
