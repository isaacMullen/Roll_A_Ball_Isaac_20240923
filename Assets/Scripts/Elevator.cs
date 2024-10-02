using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public float speed;

    public Transform pointA;
    public Transform pointB;               

    public float amplitudeOffset;
    public float amplitude;

    
    // Start is called before the first frame update   
    void Start()
    {        

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(pointA.position, pointB.position, Mathf.Sin(Time.time * speed) * amplitude + amplitudeOffset);             
    }
}
