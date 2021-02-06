using UnityEngine;

public class LastFightDialogPointController : MonoBehaviour
{
    private bool hasBeenReached = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasBeenReached) return;
        if (collision.CompareTag("Player"))
        {
            gameObject.GetComponent<LastFightDialogShowController>().StartDialog();
            hasBeenReached = true;
            collision.gameObject.GetComponent<PlayerController>().StartReading();
        }
    }
}
