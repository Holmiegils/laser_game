using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class starting : MonoBehaviour
{
    public GameObject start;
    public GameObject game;
    public int player_count;

    // Start is called before the first frame update
    void Start()
    {
        game.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player_count++;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player_count--;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player_count == 2)
        {
            game.SetActive(true);
            start.SetActive(false);
        }
    }
}
