using UnityEngine;

public class CheckPointController : MonoBehaviour
{
    private GameController gm;
    public bool isFirst;

    private void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        if (isFirst && gm.LastCheckpointPosition.x == 0 && gm.LastCheckpointPosition.y == 0) 
            gm.LastCheckpointPosition = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gm.LastCheckpointPosition = transform.position;
        }
    }
}
