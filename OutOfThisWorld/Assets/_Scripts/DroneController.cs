using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    public float max_speed = 1f;
    public float acceleration = 10f;
    public float friction = 2f;

    private Vector3 _direction = Vector3.forward;
    private float _speed = 0f;

    // Update is called once per frame
    void Update()
    {
        
    }
}
