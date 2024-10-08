using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public GameObject elevator;
    
    public GameObject pickupParent;
    public GameObject pickupParentTwo;
    
    public LayerMask groundLayer;
    public LayerMask elevatorLayer;
    public GameObject continueText;

    public float gravityMod;   
    
    public TextMeshProUGUI countText;
    
    public GameObject projectilePrefab;

    private int count;
    int amountOfPoints;
    
    private Rigidbody rb;

    private float movementX;
    private float movementY;

    public float speed = 0;
    public float jumpForce;
    bool isGrounded = true;

    GameObject pauseMenu;
    
    // Start is called before the first frame update
    void Awake()
    {
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");

        elevator.SetActive(false);
        
        pickupParentTwo.SetActive(false);
        
        Physics.gravity *= gravityMod;
        groundLayer = LayerMask.GetMask("Ground");
        
        //Debug.Log(groundLayer);
        
        continueText.SetActive(false);
        
        rb = GetComponent<Rigidbody>();
        
        count = 0;
        SetCountText();      
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.SetActive(true);
        }

        isGrounded = GroundCheck();                        
        
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && pauseMenu.activeInHierarchy == false)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }               
    }
    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x; 
        movementY = movementVector.y; 
    }
    private void FixedUpdate()
    {
        
        Vector3 movement = new Vector3 (movementX, 0.0f, movementY);
        
        rb.AddForce(movement * speed);                
    }
    void OnTriggerEnter(Collider other)
    {
        continueText.SetActive(false);
        if(other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
        }
        if(other.gameObject.CompareTag("Respawn"))
        {
            Respawn();
        }
        if(other.gameObject.CompareTag("Elevator"))
        {           
            transform.SetParent(other.transform);
            Debug.Log("PARENTED TO ELEVATOR");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        transform.SetParent(null);  
        Debug.Log("UNPARENTED TO ELEVATOR");    
    }
    void SetCountText()
    {        
        if(count >= CountChildren(pickupParent))
        {
            continueText.SetActive(true); 
            pickupParentTwo.SetActive(true); 
            elevator.SetActive(true);
            count = 0;  
            pickupParent = pickupParentTwo;      
        }
        countText.text = $"Count: {count.ToString()}/{CountChildren(pickupParent)}";
    }
    bool GroundCheck()
    {
        RaycastHit hit;
        
        if(Physics.Raycast(transform.position, Vector3.down, out hit, .75f, groundLayer))
        {                        
            return true;
            
        }
        else if(Physics.Raycast(transform.position, Vector3.down, out hit, .75f, elevatorLayer))
        {
            return true;
        }
        else
        {            
            return false;
            
        }                      
    }
    int CountChildren(GameObject gameObject) 
    {
        amountOfPoints = gameObject.transform.childCount;
        return amountOfPoints;
    }
    void Respawn()
    {
        transform.position = new Vector3(0, .5f, 3.5f);
        rb.isKinematic = true;        
        rb.isKinematic = false;
    }
    void OnFire()
    {
        if(!pauseMenu.activeInHierarchy)
        {
            FireProjectile();
        }
        
    }
    void FireProjectile()
    {
        Instantiate(projectilePrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
    }
}
