using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;
using UnityEngine.AI;
using System.Collections.Specialized;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    bool secondStageFirstLevel;
    
    public GameObject lastWall;
    
    public GameObject finishedText;
    
    public GameObject secondStretch;
    
    public Transform cameraTransform;
    
    //TIMER STUFF
    public TextMeshProUGUI timerText;
    float timer;

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
    float baseSpeed;
    public float sprintMod;
    public float jumpForce;
    bool isGrounded = true;

    GameObject pauseMenu;

    

    //SLOW TIME VARIABLES
    float slowTimeLength = 5;
    bool timeIsSlowed;

    int pointsToCollect;
    // Start is called before the first frame update
    void Awake()
    {
        secondStageFirstLevel = false;


        finishedText.SetActive(false);
        
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        
        elevator.SetActive(false);
                                    
        Physics.gravity *= gravityMod;
        groundLayer = LayerMask.GetMask("Ground");            
        
        continueText.SetActive(false);
        
        rb = GetComponent<Rigidbody>();
        
        audioSource = GetComponent<AudioSource>();
        
        

        pickupInstances = GameObject.FindGameObjectsWithTag("PickUpParent");
        foreach(GameObject p in pickupInstances)
        {
            Debug.Log("ITEM");
        }
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

        baseSpeed = speed;

        secondStretch.SetActive(false);
    }

    public void ActivateNextPickup()
    {                       
        
        foreach (GameObject pickup in pickupInstances)
        {
            if (count >= pointsToCollect && pickup.activeInHierarchy)
            {                                  
                Destroy(pickup);
                count = 0;
                if(SceneManager.GetActiveScene().buildIndex == 3)
                {
                    secondStretch.SetActive(true);
                }
                
                //Debug.Log($"children: {CountChildren(pickup)}");

            }
            
            if (!pickup.activeInHierarchy) 
            {
                pickupParent = pickup;                
                pickup.SetActive(true);
                pointsToCollect = CountChildren(pickup);
                Debug.Log($"CHILDREN: {pointsToCollect}");
                Debug.Log(pickup.name + " has been set to active.");
                SetCountText(pointsToCollect);

                break;
            }
        }
               


    }
    
    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2 && count == pointsToCollect && secondStageFirstLevel)
        {            
            lastWall.SetActive(false);
        }
        //Timer
        //Rounding to 2 decimal places and displaying them
        timerText.SetText((Mathf.Round(timer * 100) / 100).ToString("F2"));
        timer += Time.deltaTime;
        
        if(timeIsSlowed && slowTimeLength > 0)
        { 
            slowTimeLength -= Time.deltaTime;
        }
        else if(slowTimeLength <= 0 && timeIsSlowed)
        {
            Time.timeScale = 1f;
            timeIsSlowed = false;
        }

        
        
        speed = baseSpeed;
        Debug.Log(speed);
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //_-----------------------------------------------------------------------------------------------
            pauseMenu.SetActive(true);
        }

        isGrounded = GroundCheck();

        //OLD MOVEMENT--------------------------------
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && pauseMenu.activeInHierarchy == false)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }
        if(isSprinting)
        {            
            speed = baseSpeed + sprintMod;
            Debug.Log(speed);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Time.timeScale = .5f;
            timeIsSlowed = true;
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
        if (other.gameObject.CompareTag("PickUp"))
        {
            Debug.Log("HIT TARGET");
        }
        if (other.gameObject.CompareTag("Elevator"))
        {
            rb.velocity = rb.velocity - currentPlatformVelocity;
            transform.SetParent(other.transform);
            continueText.SetActive(false);
            Debug.Log("PARENTED TO ELEVATOR");
        }
        if (other.gameObject.CompareTag("Finish") && count == pointsToCollect)
        {
            Debug.Log("DONE LEVEL");
            finishedText.SetActive(true);            
            if(SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else
            {
                SceneManager.LoadScene(0);
            }
            
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
                ActivateNextPickup();
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
            //Only set elevator active if its the first level
            if(SceneManager.GetActiveScene().buildIndex == 2)
            {
                elevator.SetActive(true);
                continueText.SetActive(true);
                secondStageFirstLevel = true;
            }            
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
                Destroy(raycastHitObject);               
                count += 1;
                SetCountText(pointsToCollect);
                
            }
            SetCountText(pointsToCollect);
        }
        catch(NullReferenceException)
        {
            Debug.Log("NO OBJECT HIT");
        }

        
    }
}
