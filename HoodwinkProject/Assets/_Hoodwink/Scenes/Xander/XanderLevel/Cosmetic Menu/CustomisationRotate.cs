using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomisationRotate : MonoBehaviour
{
    float rotationSpeed = 650;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                transform.Rotate(0, Input.GetAxis("Mouse X") * -rotationSpeed * Time.deltaTime, 0, Space.World);
            }
        }
    }
}
