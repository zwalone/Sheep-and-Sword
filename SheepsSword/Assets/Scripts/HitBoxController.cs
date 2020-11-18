using UnityEngine;

public class HitBoxController : MonoBehaviour
{
    [SerializeField]
    private string target = "";

    [SerializeField]
    private int damage = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Checking tag (checking layer doesn't work):
        if (collision.CompareTag(target))
            collision.gameObject.GetComponentInParent<IEntityController>().TakeDamage(damage);
    }
}