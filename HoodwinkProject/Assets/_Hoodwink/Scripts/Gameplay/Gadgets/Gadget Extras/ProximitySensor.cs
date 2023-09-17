using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximitySensor : MonoBehaviour
{
    [SerializeField] GameObject alarm;

    private void Start()
    {
        //Find the alarm on the gadget handheld device
        alarm = GameObject.FindWithTag("Alarm");
        alarm.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            alarm.SetActive(true);
        }
    }


    /*private void OnTriggerEnter(Collider other)
    {
        //Turn on alarm when player is in proximity of the sensor
        if (other.gameObject.CompareTag("Player"))
        {
            alarm.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        //Turn it off when a player leaves
        if (other.gameObject.CompareTag("Player"))
        {
            alarm.SetActive(false);

        }
    }*/
}
