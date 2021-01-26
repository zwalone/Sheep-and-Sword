using UnityEngine;

public class DialogPointController : MonoBehaviour
{
    private bool hasBeenReached = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasBeenReached) return;
        if (collision.CompareTag("Player"))
        {
            gameObject.GetComponent<DialogShowController>().StartDialog();
            hasBeenReached = true;
            collision.gameObject.GetComponent<PlayerController>().StartReading();
        }
    }
}
