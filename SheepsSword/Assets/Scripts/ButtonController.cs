using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void RestartGame()
    {
        Debug.Log("Restarting Game...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void ReturnToMenu()
    {
        Debug.Log("Returning to Menu...");
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}