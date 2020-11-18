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
        {
            // If player is sliding - you can't hit him:
            if (target == "Player" && collision.gameObject.GetComponent<PlayerController>().IsSliding) return;

            // Apply damage:
            if (!gameObject.GetComponentInParent<IEntityController>().IsDead 
                && !gameObject.GetComponentInParent<IEntityController>().IsHurting)
                collision.gameObject.GetComponentInParent<IEntityController>().TakeDamage(damage);
        }
    }
}