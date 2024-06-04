using System.Collections;
using System.Collections.Generic;
//using System.Media;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private AudioSource backSong;
    private AudioSource gameOverSound;

    public Quaternion q;
    public bool manual;
    public UnityEngine.UI.Text gameOverText;

    void Start()
    {
        GameObject backAudioObject = GameObject.Find("BackSong");
        if (backAudioObject != null)
        {
            backSong = backAudioObject.GetComponent<AudioSource>();
        }

        GameObject gameOverAudioObject = GameObject.Find("gameOverSound");
        if (gameOverAudioObject != null)
        {
            gameOverSound = gameOverAudioObject.GetComponent<AudioSource>();
        }

        UnityEngine.Debug.Log("PlayerMovement script started.");

        if (gameOverText != null)
        {
            UnityEngine.Debug.Log("GameOverText found.");
            gameOverText.gameObject.SetActive(false);
        }
        else
        {
            UnityEngine.Debug.LogError("GameOverText reference is not set.");
        }

        if (gameOverSound != null)
        {
            gameOverSound.playOnAwake = false;
        }
    }

    public void setPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public void setRotation(Quaternion quat)
    {
        Matrix4x4 mat = Matrix4x4.Rotate(quat);
        transform.localRotation = quat;
    }

    void OnTriggerEnter(Collider other)
    {
        UnityEngine.Debug.Log("OnTriggerEnter called.");
        if (other.CompareTag("Laser"))
        {
            UnityEngine.Debug.Log("Laser hit detected.");
            ShowGameOverText();

            if (backSong != null)
            {
                backSong.Stop();
            }

            GameObject lasers = GameObject.Find("Lasers");
            GameObject buttons = GameObject.Find("buttons");
            lasers.SetActive(false);
            buttons.SetActive(false);

            AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
            foreach (AudioSource audioSource in allAudioSources)
            {
                audioSource.Stop();
            }

            if (gameOverSound != null)
            {
                gameOverSound.Play();
            }

            Invoke("EndGame", 3.0f); // Delay ending the game to show the text
        }
    }

    void ShowGameOverText()
    {
        UnityEngine.Debug.Log("ShowGameOverText called.");
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
        }
        else
        {
            UnityEngine.Debug.LogError("GameOverText is null. Cannot show game over text.");
        }
    }

    void EndGame()
    {
        UnityEngine.Debug.Log("EndGame called.");
        SceneManager.LoadScene("level_1");
    }
}
