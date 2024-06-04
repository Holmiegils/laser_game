using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class level4Manager : MonoBehaviour
{
    private AudioSource backSong;

    public buttonHandling button1;
    public buttonHandling button2;

    void Start()
    {
        GameObject audioObject = GameObject.Find("BackSong");
        if (audioObject != null)
        {
            backSong = audioObject.GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (button1.isActivated && button2.isActivated)
        {
            BothButtonsActivated();
        }
    }

    void BothButtonsActivated()
    {
        DontDestroyOnLoad(backSong);
        SceneManager.LoadScene("level_5");
    }
}