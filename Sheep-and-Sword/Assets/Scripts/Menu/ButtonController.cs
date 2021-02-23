using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    // Main-Menu buttons:
    public void StartGame() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); }
    public void ExitGame() { Application.Quit(); }



    // GameOver-Screen buttons:
    public void RestartGame() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    public void ReturnToMenu() { SceneManager.LoadScene(0); }
}
