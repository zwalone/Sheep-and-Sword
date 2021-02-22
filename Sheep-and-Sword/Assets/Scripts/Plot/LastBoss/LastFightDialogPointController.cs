using UnityEngine;

public class LastFightDialogPointController : MonoBehaviour
{
    private bool hasBeenReached = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Do nothing if it has been already reached:
        if (hasBeenReached) return;

        if (collision.CompareTag("Player"))
        {
            // Display text on the screen:
            gameObject.GetComponent<LastFightDialogShowController>().StartDialog();

            // Update dialog point status:
            hasBeenReached = true;

            // Update player state (can't be attacked while reading):
            collision.gameObject.GetComponent<PlayerController>().StartReading();
        }
    }
}
