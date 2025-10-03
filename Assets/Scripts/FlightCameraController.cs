using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlightCameraController : MonoBehaviour
{
    [Header("Required Object Components")]
    [SerializeField] Transform cameraPivot;
    [SerializeField] Transform flightModel;
    [SerializeField] Transform crossHair;

    [Header("Rotation Settings")]
    public float yawSpeed = 90f;  // Rotation speed for yaw
    public float pitchSpeed = 90f; // Rotation speed for pitch
    public float rollSpeed = 90f;  // Rotation speed for roll
    [SerializeField] float angleInDegrees = 0;
    [SerializeField] bool invertVertical = false;

    SpeedBoost boostManager;
    PlayerStats playerStatsManager;
    [SerializeField] float lerpTime = 3f;


    [Header("Lean Rotation Settings")]
    [SerializeField] float maxHorzontalLean = 60;
    [SerializeField] float maxVerticalLean = 10;
    [SerializeField] float maxCameraLean = 25;
    [SerializeField] float leanLerp = 5;
    [SerializeField] float collisionJoltTime = 20;
    [SerializeField] float collisionJoltAmount = 0.1f; 

    float yawThrow, pitchThrow, rollThrow;
    float flightSpeed;
    float joltTimer = 0;
    Quaternion joltTo;
    bool readjustInput;
    bool destroyed = false;
    private FlightCollision detector;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        boostManager = GetComponent<SpeedBoost>();
        playerStatsManager = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody>();
        detector = GetComponentInChildren<FlightCollision>();
        if (detector != null)
        {
            detector.OnCollisionWithTarget += HandleCollision;
        }
    }

    // Update is called once per frame
    void Update()
    {
        yawThrow = Input.GetAxis("Horizontal"); // xThrow
        pitchThrow = (invertVertical ? -1 : 1) * Input.GetAxis("Vertical"); //yThrow
        rollThrow = Input.GetAxis("QE");
        readjustInput = Input.GetButton("Fire2");

        Debug.DrawLine(flightModel.position, crossHair.position, Color.red);
        flightSpeed = boostManager.getFlightSpeed();
    }

    private void FixedUpdate()
    {
        if (!destroyed)
        {
            //Debug.Log(velocityVar);

            cameraPivot.position = Vector3.Lerp(cameraPivot.position, transform.position, lerpTime * Time.deltaTime);
            cameraPivot.rotation = transform.rotation;

            if (readjustInput)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.LerpAngle(transform.eulerAngles.z, 0, 5f * Time.deltaTime));
            }

            //transform.position += flightModel.forward * flightSpeed;
            rb.velocity = 30f * transform.forward * flightSpeed;

            //transform.Rotate(Vector3.up, yawThrow * yawSpeed * Time.deltaTime, Space.Self);   // Yaw (Y-axis)
            //transform.Rotate(Vector3.right, -pitchThrow * pitchSpeed * Time.deltaTime, Space.Self); // Pitch (X-axis)
            //transform.Rotate(Vector3.forward, -rollThrow * rollSpeed * Time.deltaTime, Space.Self); // Roll (Z-axis)

            Quaternion yawRot = Quaternion.Euler(new Vector3(0, yawThrow * yawSpeed * Time.deltaTime, 0));
            Quaternion pitchRot = Quaternion.Euler(new Vector3(-pitchThrow * pitchSpeed * Time.deltaTime, 0, 0));
            Quaternion rollRot = Quaternion.Euler(new Vector3(0, 0, -rollThrow * rollSpeed * Time.deltaTime));

            transform.rotation *= yawRot;
            transform.rotation *= pitchRot;
            transform.rotation *= rollRot;

            horizontalRotation(yawThrow, maxHorzontalLean, leanLerp);
            verticalRotation(pitchThrow, maxVerticalLean, leanLerp);
            joltTimer -= Time.deltaTime;
            if(joltTimer > 0)
            {
                
                transform.rotation = Quaternion.Lerp(transform.rotation, joltTo, 1 - Mathf.Exp(Time.deltaTime * -collisionJoltTime));
            }
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    void horizontalRotation(float input, float flightLeanMax, float leanLerp)
    {
        Vector3 localRot = flightModel.localEulerAngles;
        flightModel.localEulerAngles = new Vector3(localRot.x, localRot.y, Mathf.LerpAngle(localRot.z, -input * flightLeanMax, leanLerp * Time.deltaTime));

        //flightModel.localPosition = new Vector3(Mathf.LerpAngle(flightModel.localPosition.x, -yawThrow * 1.5f, lerpTime * Time.deltaTime), flightModel.localPosition.y, 0f);
        crossHair.localPosition = new Vector3(Mathf.LerpAngle(crossHair.localPosition.x, yawThrow * 3.5f, 0.5f * lerpTime * Time.deltaTime), crossHair.localPosition.y, crossHair.localPosition.z);

    }

    void verticalRotation(float input, float flightLeanMax, float leanLerp)
    {
        Vector3 localRot = flightModel.localEulerAngles;
        flightModel.localEulerAngles = new Vector3(Mathf.LerpAngle(localRot.x, -input * flightLeanMax, leanLerp * Time.deltaTime), localRot.y, localRot.z);

        crossHair.localPosition = new Vector3(crossHair.localPosition.x, Mathf.LerpAngle(crossHair.localPosition.y, input * 3f, 0.5f * lerpTime * Time.deltaTime), crossHair.localPosition.z);
    }

    void HandleCollision(Collision collided)
    {
        foreach (ContactPoint contact in collided.contacts)
        {
            Debug.Log(Vector3.Angle(contact.normal, flightModel.forward));
            float collideAngle = Vector3.Angle(contact.normal, flightModel.forward);
            Vector3 localPosition = collided.transform.InverseTransformPoint(contact.normal);
            Debug.Log(localPosition + " " + contact.normal);
            if (collideAngle > 93 && collideAngle <= 155)
            {
                Debug.DrawRay(contact.point, contact.normal * 2, Color.red);
                joltTo = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, contact.normal) + (contact.normal * collisionJoltAmount), transform.up);
                joltTimer = 0.15f;
                playerStatsManager.takeDamage(30);
                Debug.DrawRay(contact.point, (Vector3.ProjectOnPlane(transform.forward, contact.normal) + (contact.normal * 0.1f)) * 2, Color.red);
            }
            else if(collideAngle > 155 || playerStatsManager.getPlayerHealth() <= 0)
            {
                destroyed = true;
                //Debug.DrawRay(contact.point, contact.normal - Vector3.forward, Color.red);
                Quaternion rotation = Quaternion.AngleAxis(angleInDegrees, contact.normal);
                Vector3 slantedNormal = rotation * Vector3.ProjectOnPlane(Vector3.forward, contact.normal);
                Debug.DrawRay(contact.point, slantedNormal + Vector3.up, Color.red);
                Debug.Log("DEAD!");
            }

        }
        Debug.Log("Collided");
    }
}
