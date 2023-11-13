using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryAudio : MonoBehaviour
{
    public AudioSource inventoryOpen;

    void inventoryAudio()
    {
        inventoryOpen.Play();
    }
}
