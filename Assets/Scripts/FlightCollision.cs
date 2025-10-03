using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightCollision : MonoBehaviour
{
    public event Action<Collision> OnCollisionWithTarget;

    private void OnCollisionStay(Collision collision)
    {
        OnCollisionWithTarget?.Invoke(collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        //OnCollisionWithTarget?.Invoke(other.gameObject);
    }
}
