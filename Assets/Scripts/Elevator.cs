using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public Vector3 lastPosition;
    
    public float speed = 1f;
    public Transform pointA;
    public Transform pointB;

    public bool movingToB = true;
    public float progress = 0f;

    public void Start()
    {
        lastPosition = transform.position;
    }

    public void Update()
    {
        // Calculate progress based on speed and direction
        progress += (movingToB ? speed : -speed) * Time.deltaTime;

        // Clamp progress to avoid overshooting
        progress = Mathf.Clamp01(progress);

        // Move the elevator
        if(pointA != null || pointB != null)
        {
            transform.position = Vector3.Lerp(pointA.position, pointB.position, progress);
        }
        

        // Reverse direction at each end point
        if (progress >= 1f || progress <= 0f)
        {
            movingToB = !movingToB;
        }                
    }

    public Vector3 GetMovementOffset()
    {
        Vector3 offset = transform.position - lastPosition;
        lastPosition = transform.position;
        
        return offset;
    }
}