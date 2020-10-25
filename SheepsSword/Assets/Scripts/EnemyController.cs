using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HitPoint"))
            Destroy(gameObject);
    }
}
