using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_Recoil : MonoBehaviour
{
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    private float recoilForce = 10;
    private float returnSpeed = 10;

    void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, recoilForce * Time.fixedDeltaTime);

        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void AddRecoil(float verticalRecoil, float horizontalRecoil, float rotationalRecoil, float newRecoilForce, float newReturnSpeed)
    {
        recoilForce = newRecoilForce;
        returnSpeed = newReturnSpeed;
        targetRotation += new Vector3(verticalRecoil, Random.Range(-horizontalRecoil, horizontalRecoil), Random.Range(-rotationalRecoil, rotationalRecoil));
    }
}
