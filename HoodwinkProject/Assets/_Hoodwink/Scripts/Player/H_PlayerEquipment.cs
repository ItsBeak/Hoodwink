using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_PlayerEquipment : NetworkBehaviour
{

    public enum EquipmentMode
    {
        PrimaryItem,
        Sidearm,
        Holstered
    }

    public EquipmentMode currentEquipmentMode = EquipmentMode.Holstered;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void ChangeEquipmentMode(EquipmentMode mode)
    {
        currentEquipmentMode = mode;
    }
}

