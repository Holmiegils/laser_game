using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public Quaternion q;
    public bool manual;
    public UnityEngine.UI.Text gameOverText; // Make this public to assign in Inspector

    void Start()
    {
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
    }

    void Update()
    {
        // No need to log every frame unless needed for debugging
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
            Invoke("EndGame", 2.0f); // Delay ending the game to show the text
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
