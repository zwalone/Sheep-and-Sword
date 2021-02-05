using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private static GameController instance;

    // Checkpoints:
    public bool WaitingForFirstPosition = true;
    public Vector2 LastCheckpointPosition;

    // Menu showing:
    public float showSpeed;

    private void Awake()
    {
        GameObject.Find("Dialog").SetActive(false);
        GameObject.Find("GameOverText").SetActive(false);
        GameObject.Find("RestartGameButton").SetActive(false);
        GameObject.Find("GoToMenuButton").SetActive(false);
        GameObject.Find("EnemyHealthBar").SetActive(false);
        GameObject.Find("Music").GetComponents<AudioSource>()[0].ignoreListenerPause = true;

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else Destroy(gameObject);
    }

    public void GameOver()
    {
        // Show Death-Menu and PostProcessing effects:
        StartCoroutine(ShowText(GameObject.Find("UI").transform.Find("GameOverText").gameObject));
        GameObject.Find("PostProcessing").GetComponent<PostProcessingController>().ApplyPostProcessing();
        Invoke(nameof(ShowTheRest), 1.5f);

        // Change music:
        GameObject.Find("Music").GetComponents<AudioSource>()[0].volume /= 2;
        GameObject.Find("Music").GetComponents<AudioSource>()[1].Play();
    }

    private void ShowTheRest() 
    { 
        StartCoroutine(ShowImage(GameObject.Find("UI").transform.Find("RestartGameButton").gameObject));
        StartCoroutine(ShowText(GameObject.Find("UI").transform.Find("RestartGameButton").transform.Find("RestartGameText").gameObject));
        StartCoroutine(ShowImage(GameObject.Find("UI").transform.Find("GoToMenuButton").gameObject));
        StartCoroutine(ShowText(GameObject.Find("UI").transform.Find("GoToMenuButton").transform.Find("GoToMenuText").gameObject));
    }

    private IEnumerator ShowImage(GameObject hiddenObject)
    {
        hiddenObject.SetActive(true);
        Image img = hiddenObject.GetComponent<Image>();
        while (img.color.a < 1)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, img.color.a + 0.01f);
            yield return new WaitForSeconds(showSpeed);
        }
    }

    private IEnumerator ShowText(GameObject hiddenObject)
    {
        hiddenObject.SetActive(true);
        Text txt = hiddenObject.GetComponent<Text>();
        while (txt.color.a < 1)
        {
            txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, txt.color.a + 0.01f);
            yield return new WaitForSeconds(showSpeed);
        }
    }
}