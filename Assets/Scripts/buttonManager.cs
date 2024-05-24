using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using UnityEngine;

public class buttonManager : MonoBehaviour
{
    public buttonHandling button1;
    public buttonHandling button2;
    public UnityEngine.UI.Text victoryText;

    void Start()
    {
        if (victoryText != null)
        {
            victoryText.gameObject.SetActive(false);
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
        if (victoryText != null)
        {
            victoryText.gameObject.SetActive(true);
        }
        Invoke("EndGame", 2.0f);
    }

    void EndGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
