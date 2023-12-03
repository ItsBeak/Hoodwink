using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_ObjectiveMarker : MonoBehaviour
{
    public bool isSpyObjective;
    public GameObject objectiveMarker;

    private void Start()
    {
        DisableMarker();
    }

    public void EnableMarker()
    { 
        objectiveMarker.SetActive(true);
    }

    public void DisableMarker()
    { 
        objectiveMarker.SetActive(false); 
    }

    public void ToggleMarker(bool state)
    {
        objectiveMarker.SetActive(state);
    }

}
