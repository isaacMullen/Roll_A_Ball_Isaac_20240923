using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
public class PlayerController : MonoBehaviour
{
    
    public float gravityMod;
    public GameObject winTextObject;
    public TextMeshProUGUI countText;
    private int count;
    private Rigidbody rb;

    private float movementX;
    private float movementY;

    public float speed = 0;
    
    // Start is called before the first frame update
    void Start()
    {
       // Physics.gravity *= gravityMod;
        
        winTextObject.SetActive(false);
        
        rb = GetComponent<Rigidbody>();
        
        count = 0;
        SetCountText();
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     rb.velocity = new Vector3(0, 100, 0);
        // }
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
        if(other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
        }
    }
    void SetCountText()
    {
        countText.text = $"Count: {count.ToString()}";
        if(count >= 6)
        {
            winTextObject.SetActive(true);
            
        }
    }
}
