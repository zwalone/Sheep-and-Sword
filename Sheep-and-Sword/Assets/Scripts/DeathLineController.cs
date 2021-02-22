using UnityEngine;

public class DeathLineController : MonoBehaviour
{
    // After reaching this point, decrease target's health points by 1000 => kill him:
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.GetComponent<IEntityController>().TakeDamage(1000);
    }
}
