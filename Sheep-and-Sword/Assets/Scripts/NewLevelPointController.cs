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
        // Do nothing if it has been reached before:
        if (hasBeenReached) return;

        if (collision.CompareTag("Player"))
        {
            // Update NewLevelPoint status:
            hasBeenReached = true;

            // Update Game Master (it has to receive first checkpoint coords in next level):
            gm.WaitingForFirstPosition = true;

            // Start turning off the lights:
            StartCoroutine(mainCamera.GetComponent<CameraTrackController>().LightsOff());

            // Change scene:
            Invoke(nameof(NewLevel), 1.25f);
        }
    }

    private void NewLevel()
    {
        // Destroy GameMaster and his children - the checkpoints:
        Destroy(GameObject.Find("GameMaster"));

        // Load next level:
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
