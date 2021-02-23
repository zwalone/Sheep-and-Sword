using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseLevelMenu : MonoBehaviour
{
    // Load specific scene:
    public void ChooseLevel(int index)
    {
        SceneManager.LoadScene(index);
    }
}
