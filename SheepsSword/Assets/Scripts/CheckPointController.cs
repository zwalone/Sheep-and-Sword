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

    private void Awake()
    {
        gm = GameObject.Find("GameMaster").GetComponent<GameController>();
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
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasBeenReached) return;

        if (collision.CompareTag("Player"))
        {
            if (gm.LastCheckpointPosition.x != transform.position.x
                && gm.LastCheckpointPosition.y != transform.position.y)
                gameObject.GetComponent<AudioSource>().Play();

            gm.LastCheckpointPosition = transform.position;
            foreach(Transform child in transform)
            {
                if (child.CompareTag("Checkpoint_Unreached"))
                    child.gameObject.SetActive(false);
                else if (child.CompareTag("Checkpoint_Reached"))
                    child.gameObject.SetActive(true);
            }
            hasBeenReached = true;

            StartCoroutine(ShowParticles());
        }
    }

    private IEnumerator ShowParticles()
    {
        GameObject firework = Instantiate(particles,
            new Vector2(transform.position.x - particleDeltaPosition.x,
            transform.position.y - particleDeltaPosition.y), Quaternion.identity);
        firework.GetComponent<ParticleSystem>().Play();
        float ttl = firework.gameObject.GetComponent<ParticleSystem>().main.duration;
        yield return new WaitForSeconds(ttl);
        Destroy(firework);
    }
}
