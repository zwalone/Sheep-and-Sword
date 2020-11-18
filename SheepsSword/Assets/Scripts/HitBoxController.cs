using UnityEngine;

public class HitBoxController : MonoBehaviour
{
    [SerializeField]
    private string target = "";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Checking tag (checking layer doesn't work):
        if (collision.CompareTag(target))
            collision.gameObject.GetComponentInParent<IEntityController>().TakeDamage(
                gameObject.GetComponentInParent<IEntityModel>().Damage);
    }
}