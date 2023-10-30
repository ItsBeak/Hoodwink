using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_ItemSway : MonoBehaviour
{
    [Header("Components")]
    private H_PlayerController playerController;
    private CharacterController controller;

    [Header("Toggles")]
    public bool swayPosition = true;
    public bool swayRotation = true;

    Vector2 moveDir;
    Vector2 lookDir;

    [Header("Sway Settings")]
    public float smoothing = 10f;
    public float step = 0.01f;
    public float maxStepDistance = 0.06f;
    [ShowInInspector] Vector3 swayPos;

    void Start()
    {
        playerController = GetComponentInParent<H_PlayerController>();
        controller = playerController.characterController;
    }

    void Update()
    {
        if (!controller)
        {
            controller = playerController.characterController;

            if (!controller)
                return;
        }

        moveDir.x = playerController.moveDirection.x;
        moveDir.y = playerController.moveDirection.z;
        moveDir = moveDir.normalized;

        lookDir.x = Input.GetAxis("Mouse X");
        lookDir.y = Input.GetAxis("Mouse Y");

        CalculateSway();
        ApplyMovements();
    }

    void CalculateSway()
    {
        if (swayPosition == false)
        {
            swayPos = Vector3.zero;
            return;
        }

        Vector3 invertLook = lookDir * -step;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

        swayPos = invertLook;
    }

    void ApplyMovements()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, swayPos, Time.deltaTime * smoothing);
    }
}
