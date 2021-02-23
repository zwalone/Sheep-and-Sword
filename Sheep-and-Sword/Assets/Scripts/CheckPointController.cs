using System.Collections;
using UnityEngine;

public class CheckPointController : MonoBehaviour
{
    // General:
    private GameController gm;
    public bool isFirst;
    private bool hasBeenReached = false;

    // Particles:
    public GameObject particles;
    public Vector2 particleDeltaPosition;

    // Instructions for first checkpoint on the map:
    private void Awake()
    {
        gm = GameObject.Find("GameMaster").GetComponent<GameController>();
        if (isFirst && gm.WaitingForFirstPosition)
        {
            // Update Game Master:
            gm.WaitingForFirstPosition = false;
            gm.LastCheckpointPosition = transform.position;

            // Update graphics:
            foreach (Transform child in transform)
            {
                if (child.CompareTag("Checkpoint_Unreached"))
                    child.gameObject.SetActive(false);
                else if (child.CompareTag("Checkpoint_Reached"))
                    child.gameObject.SetActive(true);
            }

            // Update checkpoint state:
            hasBeenReached = true;
        }
    }

    // Instructions for reaching the checkpoint by player:
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Do nothing if it has been already reached:
        if (hasBeenReached) return;

        if (collision.CompareTag("Player"))
        {
            // Make a crow sound:
            gameObject.GetComponent<AudioSource>().Play();

            // Update Game Master:
            gm.LastCheckpointPosition = transform.position;

            // Update graphics:
            foreach(Transform child in transform)
            {
                if (child.CompareTag("Checkpoint_Unreached"))
                    child.gameObject.SetActive(false);
                else if (child.CompareTag("Checkpoint_Reached"))
                    child.gameObject.SetActive(true);
            }

            // Update checkpoint state:
            hasBeenReached = true;

            // Show black particles:
            StartCoroutine(ShowParticles());
        }
    }

    private IEnumerator ShowParticles()
    {
        GameObject par = Instantiate(particles,
            new Vector2(transform.position.x - particleDeltaPosition.x,
            transform.position.y - particleDeltaPosition.y), Quaternion.identity);
        par.GetComponent<ParticleSystem>().Play();
        float ttl = par.gameObject.GetComponent<ParticleSystem>().main.duration;
        yield return new WaitForSeconds(ttl);
        Destroy(par);
    }
}
