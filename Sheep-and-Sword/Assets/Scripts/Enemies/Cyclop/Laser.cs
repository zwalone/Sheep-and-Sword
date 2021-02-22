using System.Collections;
using UnityEngine;

public class Laser : MonoBehaviour
{
    // Value responsible for changing position:
    [SerializeField]
    private float speed = 0.0f;

    // Value responsible for decreasing target's health points:
    [SerializeField]
    private int dmg = 0;

    // Movement and animations:
    private Rigidbody2D rb2D;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        StartCoroutine(Shoot());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
        {
            // Decrease player's health points after collision with him:
            if (collision.gameObject.CompareTag("Player"))
                collision.gameObject.GetComponent<PlayerController>().TakeDamage(dmg);

            // Destroy the laser after collision with player or wall:
            if (collision.gameObject.CompareTag("Player") || collision.gameObject.layer == 8)
                Destroy(gameObject);
        }
    }

    IEnumerator Shoot()
    {
        // Animate generating laser:
        anim.Play("Laser");
        yield return new WaitForSeconds(0.07f);

        // Give some velocity to the laser:
        rb2D.velocity = transform.right * speed;

        // Destroy the laser after 5 seconds (if it won't hit anything until this time):
        Destroy(gameObject, 5);
    }
}
