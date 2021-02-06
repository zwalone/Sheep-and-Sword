using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenu;
    private PlayerController player;
    private Button pauseButton;

    private void Start()
    {
        pauseMenu.SetActive(false);
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        pauseButton = GameObject.Find("PauseButton").GetComponent<Button>();
        pauseButton.onClick.AddListener(() => PauseOrResume());
    }

    private void PauseOrResume()
    {
        if (player == null || !player.IsDead)
        {
            if (!GameIsPaused) Pause();
            else Resume();
        }
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        AudioListener.pause = true;
        GameIsPaused = true;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false;
        GameIsPaused = false;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
        AudioListener.pause = false;
        Destroy(GameObject.Find("GameMaster"));
    }
}
