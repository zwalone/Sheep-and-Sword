using System.Collections;
using UnityEngine;

public class Laser : MonoBehaviour
{

    [SerializeField]
    private float _speed = 0.5f; // JUST FOR TESTS

    [SerializeField]
    private int _dmg;

    private Rigidbody2D _rb2D;
    private Animator _anim;

    private void Awake()
    {
        _anim = this.GetComponent<Animator>();
        _rb2D = this.GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        StartCoroutine(Shoot());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Enemy"))
        {
            //TODO  HIT A PLAYER with the public function            (NOT FINAL VERSION BELOW)
            if (collision.gameObject.CompareTag("Player"))
                collision.gameObject.GetComponent<PlayerController>().TakeDamage(20); // (JUST FOR TESTING: _dmg = 20)

            Destroy(gameObject);
        }
    }

    IEnumerator Shoot()
    {
        _anim.Play("Laser");
        yield return new WaitForSeconds(0.07f);
        _rb2D.velocity = transform.right * _speed;
    }
}
