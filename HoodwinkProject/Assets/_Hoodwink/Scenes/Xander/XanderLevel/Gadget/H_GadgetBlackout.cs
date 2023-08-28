using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class H_GadgetBlackout : H_GadgetBase
{
    [Header("DimLight Settings")]
    [SerializeField] Light[] _light;
    [SerializeField] float lightIntensityDefault = 2.5f;
    [SerializeField] float lightIntensityDestination = 0.005f;
    [SerializeField] float lightTime = 0f;
    [SerializeField] int dimmedLightTime;


    bool _lerping = false;
    bool _unlerp = false;

    [Header("Audio Settings")]
    public AudioClip[] PowerOffClips;
    public AudioClip[] PowerOnClips;
    public AudioSource source;


    private void Start()
    {
        _light = (Light[])FindObjectsOfType(typeof(UnityEngine.Light));
    }
        //if (Input.GetKeyDown(KeyCode.Q))
        public override void UseGadget()
        {
            _lerping = true;
        }
    private void Update()
    {

        //Lerp all lights in array from the default intensity to the destination intensity
        if (_lerping)
        {
            foreach (Light obj in _light)
            {
            obj.intensity = Mathf.Lerp(lightIntensityDefault, lightIntensityDestination, lightTime);
            lightTime += 0.25f * Time.deltaTime;
            }
        }

        //Lerp back to the default intensity from the destination
        if (_unlerp)
        {
            foreach (Light obj in _light)
            {
                obj.intensity = Mathf.Lerp(lightIntensityDestination, lightIntensityDefault, lightTime);
                lightTime += 0.5f * Time.deltaTime;
            }
        }

        //If The intensity is less than or equal to the destination stop lerping and reset the light increments
        //Start timer
        foreach (Light obj in _light)
        {
            if (obj.intensity <= lightIntensityDestination && _lerping)
            {
                _lerping = false;
                lightTime = 0f;
                StartCoroutine(LightTimer());
            }
        }

        //If the intensity is greater than or equal to the default stop unlerping 
        foreach (Light obj in _light)

        {
            if (obj.intensity >= lightIntensityDefault && _unlerp)
            {
                _unlerp = false;
                lightTime = 0f;
            }
        }

    }
    //Dimmed light duration
    public IEnumerator LightTimer()
    {
        yield return new WaitForSeconds(dimmedLightTime);
        _unlerp = true;

    }
}

