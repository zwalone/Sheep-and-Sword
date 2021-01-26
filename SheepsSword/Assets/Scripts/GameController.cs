using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private static GameController instance;
    
    // Checkpoints:
    public bool WaitingForFirstPosition = true;
    public Vector2 LastCheckpointPosition;

    // UI and music:
    private Text gameoverText;
    private Button restartButton;
    private Button returnButton;
    private AudioSource[] gameAudioSources;

    private void Awake() 
    {
        gameoverText = GameObject.Find("GameOverText").GetComponent<Text>();
        restartButton = GameObject.Find("RestartGameButton").GetComponent<Button>();
        returnButton = GameObject.Find("GoToMenuButton").GetComponent<Button>();
        gameoverText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        returnButton.gameObject.SetActive(false);
        GameObject.Find("EnemyHealthBar").SetActive(false);

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
        gameoverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        returnButton.gameObject.SetActive(true);

        gameAudioSources[0].volume /= 2;
        gameAudioSources[1].Play();
    }
}