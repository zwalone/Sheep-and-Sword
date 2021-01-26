using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController instance;
    
    // Checkpoints:
    public bool WaitingForFirstPosition = true;
    public Vector2 LastCheckpointPosition;

    // UI and music:
    private GameObject dialog;
    private GameObject gameoverText;
    private GameObject restartButton;
    private GameObject returnButton;
    private GameObject enemyHealthBar;
    private AudioSource[] gameAudioSources;

    private void Awake()
    {
        dialog = GameObject.Find("Dialog");
        gameoverText = GameObject.Find("GameOverText");
        restartButton = GameObject.Find("RestartGameButton");
        returnButton = GameObject.Find("GoToMenuButton");
        enemyHealthBar = GameObject.Find("EnemyHealthBar");
        dialog.SetActive(false);
        gameoverText.SetActive(false);
        restartButton.SetActive(false);
        returnButton.SetActive(false);
        enemyHealthBar.SetActive(false);
        gameAudioSources = GameObject.Find("Music").GetComponents<AudioSource>();

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else Destroy(gameObject);
    }

    public void GameOver()
    {
        gameoverText.SetActive(true);
        restartButton.SetActive(true);
        returnButton.SetActive(true);

        gameAudioSources[0].volume /= 2;
        gameAudioSources[1].Play();
    }
}