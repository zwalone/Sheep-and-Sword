using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    // Menu Buttons:
    public void StartGame() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); }
    public void ExitGame() { Application.Quit(); }



    // In-game buttons:
    public void RestartGame() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    public void ReturnToMenu() { SceneManager.LoadScene(0); }
}
