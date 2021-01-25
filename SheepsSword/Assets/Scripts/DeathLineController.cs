using UnityEngine;

public class DeathLineController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.GetComponent<IEntityController>().TakeDamage(1000);
    }
}
