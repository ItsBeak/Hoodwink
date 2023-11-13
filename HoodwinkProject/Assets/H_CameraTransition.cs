using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class H_CameraTransition : MonoBehaviour
{
    public CinemachineBrain brain;

    void Start()
    {
        brain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
        brain.m_DefaultBlend.m_Time = 2f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
