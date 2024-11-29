using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor;
using Unity.VisualScripting;


public class ScoreScreen : MonoBehaviour
{
    public float currentLevelPerfect;
    private string currentLevel;

    public PlayerController playerController; 
    
    public TextMeshProUGUI scoreText;
    public GameObject scorePanel;
    public GameObject gamePanel;    
   
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

    public void DetermineScore()
    {
        float time = playerController.timer;

        AdaptPerfectScore();

        if (time < currentLevelPerfect)
        {
            scoreText.SetText($"Time {time.ToString("F2")} | A+ Rating");
        }
        else if(time < currentLevelPerfect * 1.15f)
        {
            scoreText.SetText($"Time {time.ToString("F2")} | A Rating");
        }
        else if(time < currentLevelPerfect * 1.25f)
        {
            scoreText.SetText($"Time {time.ToString("F2")} | B Rating");
        }
        else if(time < currentLevelPerfect * 1.4f)
        {
            scoreText.SetText($"Time {time.ToString("F2")} | C Rating");
        }
        else if(time < currentLevelPerfect * 1.6f)
        {
            scoreText.SetText($"Time {time.ToString("F2")} | D Rating");
        }
        else
        {
            scoreText.SetText("Yikes");
        }
        

        StartCoroutine(HandleScoreDisplay());
    }   
    
    private IEnumerator HandleScoreDisplay()
    {                
        gamePanel.SetActive(false);
        scorePanel.SetActive(true);

        Time.timeScale = 0;

        yield return new WaitUntil(() => Input.anyKeyDown);

        
        gamePanel.SetActive(true);
        scorePanel.SetActive(false);

        if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            Time.timeScale = 1f;
        }
        else
        {
            SceneManager.LoadScene(0);
            Time.timeScale = 1f;
        }
    }
}
