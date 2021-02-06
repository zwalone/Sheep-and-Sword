using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseLevelMenu : MonoBehaviour
{
    public void ChooseLevel(int index)
    {
        SceneManager.LoadScene(index);
    }
}
