using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class H_GadgetProximitySensor : H_GadgetBase
{
    [Header("Proximity Settings")]
    [SerializeField] GameObject proximitySensor;
    [SerializeField] Animator proximityMenu;
    [SerializeField] GameObject onLight;

    GameObject tempSensor;
    bool proxyActive;
    bool instantiated;
    private void Start()
    {
        instantiated = false;
        onLight.SetActive(false);
    }
    public override void UseGadget()
    {
        if (!proxyActive)
        {
            proximityMenu.SetTrigger("proxyOn");
        }
        else
        {
            proximityMenu.SetTrigger("proxyOff");
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !instantiated && proxyActive)
        {
            Vector3 newpos = gameObject.transform.position;
            GameObject sensor = Instantiate(proximitySensor, newpos, Quaternion.identity);
            instantiated = true;
            onLight.SetActive(true);
            tempSensor = sensor;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && instantiated && proxyActive)
        {
            Debug.Log("Yipee");
            Vector3 repos = gameObject.transform.position;
            tempSensor.transform.position = repos;
            Debug.Log("PosChange");
        }
    }

    void proximityUp()
    {
        proxyActive = true;
    }
    void proximityDown()
    {
        proxyActive = false;
    }
}
