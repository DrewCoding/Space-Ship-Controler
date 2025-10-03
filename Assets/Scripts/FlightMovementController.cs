using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightMovementController : MonoBehaviour
{
    [SerializeField] Transform cameraPivot;
    [SerializeField] Transform flightModel;
    [SerializeField] float inputHorizSensitivity = 130f;
    [SerializeField] float inputVertSensitivity = 50f;
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float brakeSpeed = 3f;
    [SerializeField] float boostSpeed = 10f;
    [SerializeField] float lerpTime = 0;
    [SerializeField] bool invertVertical = false;
    [SerializeField] float maxVerticalClamp = 80f;
    [SerializeField] float minVerticalClamp = -80f;

    [Space]
    [SerializeField] float maxHorzontalLean = 60;
    [SerializeField] float maxVerticalLean = 10;
    [SerializeField] float maxCameraLean = 25;
    [SerializeField] float leanLerp = 5;
    float xThrow, yThrow;
    float xRot = 0, yRot = 0;
    float lerpTilt = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        xThrow = Input.GetAxis("Horizontal");
        yThrow = (invertVertical ? 1 : -1) * Input.GetAxis("Vertical");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += transform.forward * movementSpeed * 0.1f;
        cameraPivot.position = Vector3.Lerp(cameraPivot.position, transform.position, lerpTime * Time.deltaTime);

        cameraPivot.rotation = transform.rotation;

        steeringControls();
        horizontalRotation(xThrow, maxHorzontalLean, leanLerp);
        verticalRotation(yThrow, maxVerticalLean, leanLerp);
    }

    void steeringControls()
    {
        xRot += yThrow * inputVertSensitivity * Time.deltaTime;
        yRot += xThrow * inputHorizSensitivity * Time.deltaTime;

        xRot = Mathf.Clamp(xRot, minVerticalClamp, maxVerticalClamp);

        Quaternion newRot = Quaternion.Euler(xRot, yRot, 0f);
        transform.localRotation = newRot;

    }

    void horizontalRotation(float input, float flightLeanMax, float leanLerp)
    {
        Vector3 localRot = flightModel.localEulerAngles;
        flightModel.localEulerAngles = new Vector3(localRot.x, localRot.y, Mathf.LerpAngle(localRot.z, -input * flightLeanMax, leanLerp * Time.deltaTime));

        lerpTilt = Mathf.LerpAngle(lerpTilt, maxCameraLean * xThrow, 5f * Time.deltaTime);
        Debug.Log(lerpTilt);
    }

    void verticalRotation(float input, float leanMax, float leanLerp)
    {
        Vector3 localRot = flightModel.localEulerAngles;
        flightModel.localEulerAngles = new Vector3(Mathf.LerpAngle(localRot.x, input * leanMax, leanLerp * Time.deltaTime), localRot.y, localRot.z);
    }
}
