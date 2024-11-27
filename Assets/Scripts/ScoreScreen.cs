using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class ScoreScreen : MonoBehaviour
{
    public float currentLevelPerfect;
    private string currentLevel;

    public PlayerController playerController; 
    
    public TextMeshProUGUI scoreText;
    public GameObject scorePanel;
    public GameObject gamePanel;

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    private void OnEnable()
    {
        // Subscribe to the event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid issues
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {                
        currentLevel = SceneManager.GetActiveScene().name;
        Debug.Log($"Current Scene Name: {currentLevel}");
    }

    public void AdaptPerfectScore()
    {
        if (currentLevel == "levelOne")
        {
            currentLevelPerfect = 12f;
        }
        else if(currentLevel == "levelTwo")
        {
            currentLevelPerfect = 15f;
        }
    }

    public void ShowScoreScreen()
    {
        float time = playerController.timer;

        AdaptPerfectScore();

        if (time < currentLevelPerfect)
        {
            Debug.Log("A+ Rating");
        }
        else if(time < currentLevelPerfect * 1.15f)
        {
            Debug.Log("A Rating");
        }
        else if(time < currentLevelPerfect * 1.25f)
        {
            Debug.Log("B Rating");
        }
        else if(time < currentLevelPerfect * 1.4f)
        {
            Debug.Log("C Rating");
        }
        else if(time < currentLevelPerfect * 1.6f)
        {
            Debug.Log("D Rating");
        }
        else
        {
            Debug.Log("Yikes");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
