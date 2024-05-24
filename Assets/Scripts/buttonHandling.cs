using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonHandling : MonoBehaviour
{
    public Sprite greenSprite; // Reference to the green sprite
    public Sprite redSprite; // Reference to the red sprite
    public bool isActivated = false; // Track the activation state

    private SpriteRenderer spriteRenderer; // Reference to the sprite renderer

    void Start()
    {
        // Get the sprite renderer component
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Ensure the red sprite is set initially
        if (spriteRenderer != null && redSprite != null)
        {
            spriteRenderer.sprite = redSprite;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && spriteRenderer != null && greenSprite != null)
        {
            // Change to green sprite
            spriteRenderer.sprite = greenSprite;
            isActivated = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && spriteRenderer != null && redSprite != null)
        {
            // Change to red sprite
            spriteRenderer.sprite = redSprite;
            isActivated = false;
        }
    }
}
