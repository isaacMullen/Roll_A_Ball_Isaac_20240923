using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;
using UnityEngine.AI;
using System.Collections.Specialized;

public class PlayerController : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip clip;
    
    GameObject raycastHitObject;

    public Transform gunPoint;
    
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
    GameObject projectile;
    public float projectileSpeed;

    public int count;
    int amountOfPoints;
    
    private Rigidbody rb;

    private float movementX;
    private float movementY;

    //PLAYER MOVE SPEED
    public float speed = 0;
    public float jumpForce;
    bool isGrounded = true;

    GameObject pauseMenu;

    int pointsToCollect;
    
    // Start is called before the first frame update
    void Awake()
    {
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");

        elevator.SetActive(false);            
        
        Physics.gravity *= gravityMod;
        groundLayer = LayerMask.GetMask("Ground");            
        
        continueText.SetActive(false);
        
        rb = GetComponent<Rigidbody>();
        
        audioSource = GetComponent<AudioSource>();
        
        

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
        int pointsToCollect = CountChildren(GameObject.FindWithTag("PickUpParent"));

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
            if (count >= pointsToCollect && pickup.activeInHierarchy)
            {                                  
                Destroy(pickup);
                count = 0;
                Debug.Log(CountChildren(pickup));
                pointsToCollect = CountChildren(pickup);
            }
            
            if (!pickup.activeInHierarchy) 
            {
                pickupParent = pickup;                
                pickup.SetActive(true);
                pointsToCollect = CountChildren(pickup);
                Debug.Log(pickup.name + " has been set to active.");
                SetCountText(CountChildren(pickupParent));
                break;
            }
        }
               


    }
    
    // Update is called once per frame
    void Update()
    {       
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.SetActive(true);
        }

        isGrounded = GroundCheck();

        //OLD MOVEMENT--------------------------------
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && pauseMenu.activeInHierarchy == false)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }

    }
    //OLD MOVEMENT--------------------------------
    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;
    }
    private void FixedUpdate()
    {

        //OLD MOVEMENT--------------------------------
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);

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
                if (count >= pointsToCollect)
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
    
    
    public void SetCountText(int children)
    {
        //if all objects are picked up ACTIVATING ELEVATOR
        if (ArePointsCollected() && elevator.activeInHierarchy == false)
        {                        
            elevator.SetActive(true);
            continueText.SetActive(true);
        }
        countText.text = $"Count: {count.ToString()}/{children}";
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
    public int CountChildren(GameObject gameObject) 
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
        Shoot();
        magazineSize.bulletsRemaining--;
    }

    void Shoot()
    {
        Camera cam = Camera.main;
        Vector3 cameraPosition = cam.transform.position;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        Vector3 targetPoint;

        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(1000);
        }

        Vector3 direction = (targetPoint - gunPoint.position).normalized;
        projectile = Instantiate(projectilePrefab, gunPoint.position, Quaternion.identity);
        //PLAYSOUND HERE---------------------------------------
        //audioSource.Play();


        Rigidbody proRb = projectile.GetComponent<Rigidbody>();
        proRb.velocity = direction * projectileSpeed;

        try
        {
            raycastHitObject = hit.collider.gameObject;
            if (raycastHitObject.CompareTag("PickUp"))
            {

                Debug.Log("HIT PICKUP");
                count += 1;
                SetCountText(pointsToCollect);
                Destroy(raycastHitObject);
            }
        }
        catch(NullReferenceException)
        {
            Debug.Log("NO OBJECT HIT");
        }

        
    }
}
