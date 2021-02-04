using UnityEngine;
using UnityEngine.SceneManagement;

public class NewLevelPointController : MonoBehaviour
{
    private GameController gm;
    private GameObject mainCamera;
    private bool hasBeenReached = false;

    private void Awake()
    {
        mainCamera = GameObject.Find("Main Camera");
        gm = GameObject.Find("GameMaster").GetComponent<GameController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasBeenReached) return;
        if (collision.CompareTag("Player"))
        {
            hasBeenReached = true;
            gm.WaitingForFirstPosition = true;
            StartCoroutine(mainCamera.GetComponent<CameraTrackController>().LightsOff());
            Invoke(nameof(NewLevel), 1.25f);
        }
    }

    private void NewLevel()
    {
        Destroy(GameObject.Find("GameMaster"));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
