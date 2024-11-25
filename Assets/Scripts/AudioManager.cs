using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    AudioClip clip;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        
        clip = audioSource.GetComponent<AudioSource>().clip;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
