using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.GlobalIllumination;

public class SpeedBoost : MonoBehaviour
{
    [SerializeField] Transform flightModel;

    [Header("Speed Settings")]
    public float normalSpeed = 0.3f;
    public float boostSpeed = 0.8f;
    public float brakeSpeed = 0.15f;
    public float boostRechargeTime = 3f;
    public float boostTime = 3f;

    float flightSpeed;
    float _boostTime = 3f, _boostRechargeTimer = 0;
    bool canBoost = true;
    float velocityVar = 0;
    Vector3 displacement = Vector3.zero;
    bool boostInput, brakeInput;

    void Start()
    {
        flightSpeed = normalSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        boostInput = Input.GetButton("Fire1");
        brakeInput = Input.GetButton("Fire3");
    }

    private void FixedUpdate()
    {
        velocityVar = ((flightModel.position - displacement) / Time.deltaTime).magnitude;
        displacement = flightModel.position;

        if (boostInput)
        {
            flightBoost();
        }
        else if (brakeInput)
        {
            flightSpeed = brakeSpeed;
        }
        else
        {
            flightSpeed = normalSpeed;
        }

        //Debug.Log(_boostTime + " " + canBoost);

        flightRecharge();
    }

    public float getFlightSpeed()
    {
        return flightSpeed;
    }

    void flightBoost()
    {
        _boostRechargeTimer = 0;
        if (velocityVar > 28f && canBoost) // 28 being the velocity 28 m/s, which is the current speed set by the speed values. May need to change if the speed values are changed.
        {
            flightSpeed = boostSpeed;
            _boostTime -= Time.deltaTime;
        }
        else
        {
            flightSpeed = normalSpeed;
        }
    }

    void flightRecharge()
    {
        if (_boostRechargeTimer >= boostRechargeTime)
        {
            canBoost = true;
            _boostTime += Time.deltaTime;
            _boostTime = Mathf.Clamp(_boostTime, 0, boostTime);
        }

        if (_boostTime <= 0)
        {
            canBoost = false;
            _boostRechargeTimer += Time.deltaTime;
        }
    }
}
