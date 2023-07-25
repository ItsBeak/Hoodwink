using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class H_ThingDoer : MonoBehaviour
{
    public UnityEvent thingsToDo;

    public void DoThings()
    {
        thingsToDo.Invoke();
    }

}
