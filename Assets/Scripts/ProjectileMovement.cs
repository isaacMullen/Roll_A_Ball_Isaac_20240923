using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{        
    PlayerController playerScript;
    
    public float speed;

    GameObject player;
    Transform playerPosition;

    Vector3 mousePosition;

    Rigidbody playerRb;
    Rigidbody rb;

    Vector3 movementDirection;
    Vector3 mouseWorldPosition;

    float cameraDepth = 14.5f;
    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
       // playerPosition = player.transform;
        
        //playerRb = player.GetComponent<Rigidbody>();
        rb = GetComponent<Rigidbody>();

        //Getting Mouse Position
        mousePosition = Input.mousePosition;

        //Finding 3D world position of the mouse at a specified depth from the camera
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, cameraDepth));
    }
    void Start()
    {
        //Rotating my object from its original position to the mouse's position
        movementDirection = (mouseWorldPosition - transform.position).normalized;
        
        playerScript = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {                
        rb.velocity = movementDirection * speed * Time.deltaTime;

        
        if(Distance() > 30f)
        {
            Destroy(this.gameObject);
        }

    }
    float Distance()
    {
        //Getting distance between projectile and player so i can destroy projectile
        return Vector3.Distance(transform.position, player.transform.position);
    }
    
}
