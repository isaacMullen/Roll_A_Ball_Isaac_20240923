using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{       

    public float speed;

    Rigidbody playerRb;
    Rigidbody rb;

    Vector3 movementDirection;
    // Start is called before the first frame update
    void Awake()
    {  
        playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();         
    }
    void Start()
    {                                
        rb = GetComponent<Rigidbody>();
        movementDirection = playerRb.velocity.normalized;
    }

    // Update is called once per frame
    void Update()
    {     
        rb.velocity = movementDirection * speed * Time.deltaTime;
    }
}
