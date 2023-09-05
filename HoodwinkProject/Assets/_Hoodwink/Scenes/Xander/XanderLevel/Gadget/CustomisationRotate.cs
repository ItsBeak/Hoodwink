using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomisationRotate : MonoBehaviour
{
    [SerializeField] float rotationSpeed;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * -rotationSpeed * Time.deltaTime, 0, Space.World);
        }
    }
}
