using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject crosshair;
    
    GameObject pauseMenu;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu = GameObject.FindWithTag("PauseMenu");
        pauseMenu.SetActive(false);

        DontDestroyOnLoad(this);
        
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
        crosshair.SetActive(false);
        Time.timeScale = 0;
        Cursor.visible = true;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
        crosshair.SetActive(true);
        Cursor.visible = false;
    }
    public void OnResumeButton()
    {
        ResumeGame();
        gameObject.SetActive(false);
    }
}
