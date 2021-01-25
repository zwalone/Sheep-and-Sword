using UnityEngine;

public class CheckPointController : MonoBehaviour
{
    private GameController gm;
    public bool isFirst;
    private bool hasBeenReached = false;

    private void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        if (isFirst && gm.WaitingForFirstPosition)
        {
            gm.WaitingForFirstPosition = false;
            gm.LastCheckpointPosition = transform.position;
            foreach (Transform child in transform)
            {
                if (child.CompareTag("Checkpoint_Unreached"))
                    child.gameObject.SetActive(false);
                else if (child.CompareTag("Checkpoint_Reached"))
                    child.gameObject.SetActive(true);
            }
            hasBeenReached = true;
            gameObject.GetComponent<AudioSource>().Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasBeenReached) return;

        if (collision.CompareTag("Player"))
        {
            gm.LastCheckpointPosition = transform.position;
            foreach(Transform child in transform)
            {
                if (child.CompareTag("Checkpoint_Unreached"))
                    child.gameObject.SetActive(false);
                else if (child.CompareTag("Checkpoint_Reached"))
                    child.gameObject.SetActive(true);
            }
            hasBeenReached = true;
            gameObject.GetComponent<AudioSource>().Play();
        }
    }
}
