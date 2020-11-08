using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{

    [SerializeField]
    private float _speed;

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
        if(collision.tag != "Enemy")
        {
            //TODO  HIT A PLAYER with the public function 
            
            //GameObject obj = collision.GetComponent<Player>();
            //if (obj != null) obj.TakeDamage(_dmg);
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
