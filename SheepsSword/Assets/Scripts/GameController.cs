﻿using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController instance;
    
    // Checkpoints:
    public bool WaitingForFirstPosition = true;
    public Vector2 LastCheckpointPosition;

    private void Awake()
    {
        WaitingForFirstPosition = true;
        GameObject.Find("Dialog").SetActive(false);
        GameObject.Find("GameOverText").SetActive(false);
        GameObject.Find("RestartGameButton").SetActive(false);
        GameObject.Find("GoToMenuButton").SetActive(false);
        GameObject.Find("EnemyHealthBar").SetActive(false);

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else Destroy(gameObject);
    }

    public void GameOver()
    {
        GameObject.Find("UI").transform.Find("GameOverText").gameObject.SetActive(true);
        GameObject.Find("UI").transform.Find("RestartGameButton").gameObject.SetActive(true);
        GameObject.Find("UI").transform.Find("GoToMenuButton").gameObject.SetActive(true);
        GameObject.Find("Music").GetComponents<AudioSource>()[0].volume /= 2;
        GameObject.Find("Music").GetComponents<AudioSource>()[1].Play();
    }
}