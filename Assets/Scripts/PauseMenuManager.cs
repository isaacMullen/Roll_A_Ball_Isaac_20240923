using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    GameObject pauseMenu;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu = GameObject.FindWithTag("PauseMenu");
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    public void Update()
    {        
        if(pauseMenu.activeInHierarchy)
        {
            PauseGame();
        }                                
    }
    public void PauseGame()
    {      
        Time.timeScale = 0;       
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
    public void OnResumeButton()
    {
        ResumeGame();
        gameObject.SetActive(false);
    }
}
