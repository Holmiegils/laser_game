using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class level1Manager : MonoBehaviour
{
    public AudioSource backSong;
    public buttonHandling button1;
    public buttonHandling button2;

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
        SceneManager.LoadScene("level_2");
    }
}
