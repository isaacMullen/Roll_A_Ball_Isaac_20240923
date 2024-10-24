using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;

public class PlayerController : MonoBehaviour
{
    public GameObject magazine;
    MagazineSize magazineSize;
    
    public GameObject elevator;
    Vector3 previousPlatformPosition;
    Vector3 currentPlatformVelocity;
    
    GameObject pickupParent;
    public GameObject[] pickupInstances;
    
    public LayerMask groundLayer;
    public LayerMask elevatorLayer;
    public GameObject continueText;

    public float gravityMod;   
    
    public TextMeshProUGUI countText;
    
    public GameObject projectilePrefab;

    public int count;
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
        
        Physics.gravity *= gravityMod;
        groundLayer = LayerMask.GetMask("Ground");            
        
        continueText.SetActive(false);
        
        rb = GetComponent<Rigidbody>();
        
        
        

        pickupInstances = GameObject.FindGameObjectsWithTag("PickUpParent");
        System.Array.Sort(pickupInstances, (a, b) => a.name.CompareTo(b.name));

        foreach(var pickupInstance in pickupInstances)
        {
            Debug.Log(pickupInstance.name);
        }

        foreach(GameObject pickup in  pickupInstances)
        {
            pickup.SetActive(false);
        }
        ActivateNextPickup();
        count = 0;
        SetCountText();

    }
    
    private void Start()
    {
        previousPlatformPosition = elevator.transform.Find("Collider").transform.position;
        magazineSize = magazine.GetComponent<MagazineSize>();        
    }

    public void ActivateNextPickup()
    {
        
        foreach (GameObject pickup in pickupInstances)
        {
            if (count >= CountChildren(pickup) && pickup.activeInHierarchy)
            {
                Destroy(pickup);
                count = 0;
            }
            Debug.Log("Checking " + pickup.name + " | Active: " + pickup.activeInHierarchy);
            if (!pickup.activeInHierarchy)
            { 
                pickupParent = pickup;  
                pickup.SetActive(true);
                Debug.Log(pickup.name + " has been set to active.");
                break;
            }            
        }        
    }
    
    // Update is called once per frame
    void Update()
    {
        //ArePointsCollected();
        
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

        //Getting velocity of the platform using transforms instead of rigidbody
        currentPlatformVelocity = (elevator.transform.Find("Collider").transform.position - previousPlatformPosition) / Time.deltaTime;
        //Updating the position of the platform
        previousPlatformPosition = elevator.transform.Find("Collider").transform.position;
    }
    void OnTriggerEnter(Collider other)
    {
             
        if(other.gameObject.CompareTag("Respawn"))
        {
            Respawn();
        }
        if(other.gameObject.CompareTag("Elevator"))
        {
            rb.velocity = rb.velocity - currentPlatformVelocity;
            transform.SetParent(other.transform);
            continueText.SetActive(false);
            Debug.Log("PARENTED TO ELEVATOR");
        }
    }
    private void OnTriggerExit(Collider other)
    {        
        if(other.gameObject.CompareTag("Elevator"))
        {
            transform.SetParent(null);
            Debug.Log("UNPARENTED TO ELEVATOR");
        }
              
    }
    
    //CHECKING IF ALL POINTS ARE COLLECTED. CALLED IN THE ProjectileMovement SCRIPT
    public bool ArePointsCollected()
    {
        try
        {
            if(CountChildren(pickupParent) != 0)
            {
                if (count >= CountChildren(pickupParent))
                {
                    ActivateNextPickup();                                                            
                    return true;

                }
               
                else
                {                    
                    return false;
                }                
            }
            else
            {
                pickupParent = GameObject.FindWithTag("PickUpParent");
                pickupParent.SetActive(true);
                //IF THE COUNT CHILDREN IS NOT 0 IT RETURNS TRUE
                return true;
            }
                       
        }
        catch(MissingReferenceException)
        {
            Debug.Log("No PickUps in Scene");
            return true;
        }        
    }
    
    
    public void SetCountText()
    {
        //if all objects are picked up ACTIVATING ELEVATOR
        if (ArePointsCollected() && elevator.activeInHierarchy == false)
        {                        
            elevator.SetActive(true);
            continueText.SetActive(true);
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
        
        if(!pauseMenu.activeInHierarchy && magazineSize.bulletsRemaining > 0 && magazineSize.reload == 2) 
            FireProjectile();
    }
    void FireProjectile()
    {
        Instantiate(projectilePrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        magazineSize.bulletsRemaining--;
    }
}
