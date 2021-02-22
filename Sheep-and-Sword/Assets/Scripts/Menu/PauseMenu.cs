using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenu;
    private PlayerController player;

    private void Start()
    {
        pauseMenu.SetActive(false);
        player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && (player == null || !player.IsDead))
        {
            if (!GameIsPaused) Pause();
            else Resume();
        }
    }

    // Freeze movement and sounds, show pauseMenu
    private void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        AudioListener.pause = true;
        GameIsPaused = true;
    }

    // Unfreeze movement and sounds, hide pauseMenu
    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false;
        GameIsPaused = false;
    }

    // Quit the application
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
