using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BarrelController3D : MonoBehaviour
{
    public Transform ball;
    private Camera mainCamera;
    public LayerMask targetLayer;

    public Vector3 offset;
    public float planeOffset;

    // Start is called before the first frame update
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");

        mainCamera = Camera.main;

        if(mainCamera == null)
        {
            Debug.Log("Main Camera Not Found");
        }
        try
        {
            ball = GameObject.Find("Player").transform;
        }
        catch(NullReferenceException)
        {
            Debug.Log("Can't Find Ball");
        }
        
    }

    void Start()
    {
        DontDestroyOnLoad(this);        
    }

    // Update is called once per frame
    void Update()
    {
        if(ball == null)
        {
            return;
        }

        //transform.position = ball.position + offset;

        Vector3 mousePosition = Input.mousePosition;

        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        Vector3 planePosition = ball.position + mainCamera.transform.forward * planeOffset;
        Plane cameraPlane = new Plane(-mainCamera.transform.forward, planePosition);
        
        if(cameraPlane.Raycast(ray, out float distance))
        {
            Vector3 targetPoint = ray.GetPoint(distance);

            Vector3 direction = (targetPoint - ball.position).normalized;

            transform.position = ball.position + direction * 1f;

            Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0);

            // Apply the rotation
            transform.rotation = targetRotation;
        }
        
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
    }
}
