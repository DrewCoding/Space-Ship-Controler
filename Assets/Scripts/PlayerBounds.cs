using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBounds : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField] float placeBackToBounds = 0.999f;
    // y
    [SerializeField] float upperBound = 100f;
    [SerializeField] float lowerBound = -100f;
    // x
    [SerializeField] float frontBound = 100f;
    [SerializeField] float backBound = -100f;
    // z
    [SerializeField] float rightBound = 100f;
    [SerializeField] float leftBound = -100f;

    [SerializeField] Vector3[] pointToPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x > frontBound || transform.position.x < backBound || transform.position.y > upperBound || transform.position.y < lowerBound || transform.position.z > rightBound || transform.position.z < leftBound)
        {
            Debug.Log("Out of bounds");
            Vector3 closestToPointTo = Vector3.zero;
            Vector3 positionOfOOB = transform.position;
            foreach (Vector3 rp in pointToPosition)
            {
                if(Vector3.Distance(transform.position, rp) < Vector3.Distance(transform.position, closestToPointTo))
                {
                    closestToPointTo = rp;
                }
            }
            transform.position = positionOfOOB * placeBackToBounds;
            Debug.Log(positionOfOOB * placeBackToBounds);
            transform.rotation = Quaternion.LookRotation(closestToPointTo - transform.position);
        } 
    }
}
