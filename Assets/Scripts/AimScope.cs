using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimScope : MonoBehaviour
{
    // Start is called before the first frame update
    Camera cam;
    [SerializeField] Transform focus;
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 objPos = cam.WorldToScreenPoint(focus.position);

        transform.position = objPos;
    }
}
