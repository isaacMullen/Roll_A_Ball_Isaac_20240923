using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MagazineSize : MonoBehaviour
{
    public Slider magazine;
    public TextMeshProUGUI shotsRemainingText;
    
    int currentLevel;

    public int bulletsRemaining;
    public int magazineSize = 0;

    public float reload = 2;

    
    // Start is called before the first frame update
    void Awake()
    {
        currentLevel = SceneManager.GetActiveScene().buildIndex;
    }
    void Start()
    {        
        magazineSize = 30;
        bulletsRemaining = magazineSize;
        
        magazine.maxValue = magazineSize;
        magazine.value = magazine.maxValue;
        
        UpdateBulletText(magazineSize);
    }


    // Update is called once per frame
    void Update()
    {
        magazine.value = bulletsRemaining;

        UpdateBulletText(bulletsRemaining);
        
        if (bulletsRemaining == 0)
        {
            reload -= Time.deltaTime;
            
            if(reload < 0)
            {
                reload = 2;
                bulletsRemaining = magazineSize;
            }
        }
        Debug.Log(reload);
    }
    void UpdateBulletText(int bullets)
    {
        shotsRemainingText.text = bullets.ToString();
    }
}
