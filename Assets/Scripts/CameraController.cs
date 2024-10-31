using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{     


    
    public GameObject player;
    private Quaternion initialRotation;

    public Vector3 offset;
    
    // Start is called before the first frame update
    void Start()
    {
        initialRotation = transform.rotation;
    }

    // Update is called once per frame
    void LateUpdate()
    {       
        transform.SetPositionAndRotation(player.transform.position + offset, initialRotation);
    }
}
