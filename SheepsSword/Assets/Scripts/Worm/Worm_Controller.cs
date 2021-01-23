using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm_Controller : MonoBehaviour, IEntityController
{
    public Transform rayCast;
    public LayerMask rayCastMask;
    public float rayCastLength;
    public float attackDistance;

    private GameObject target;
    private bool _inRange;

    private Worm_Model _model;
    private Worm_View _view;

    private Rigidbody2D _rd2D;

    [SerializeField]
    private GameObject hitbox;

    [SerializeField]
    private CircleCollider2D _isGroundBottom;

    [SerializeField]
    private CircleCollider2D _isGroundOpposite;


    // Parameters:
    [SerializeField]
    private bool _changeDirection;
    private bool _isAttacking;

    public bool IsHurting { get; private set; }
    public bool IsDead { get; private set; }




    private void Awake()
    {
        _view = this.GetComponent<Worm_View>();
        _model = this.GetComponent<Worm_Model>();
        _rd2D = this.GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        _changeDirection = true;
    }

    private void FixedUpdate()
    {
        //Move Enemy and check direction
        _rd2D.MovePosition(_rd2D.position + new Vector2(_model.Speed, 0) * Time.fixedDeltaTime);
        ChangeMoveDirection();
    }

    private void Update()
    {
        Animate();
        if (_inRange) CheckAttack();
    }



    //Attack
    private void CheckAttack()
    {
        if (!_isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player")
            && !collider.gameObject.GetComponentInParent<IEntityController>().IsDead)
        {
            _inRange = true;
            target = collider.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _inRange = false;
            target = null;
        }
    }

    // Coroutine for Attack
    IEnumerator Attack()
    {
        hitbox.GetComponent<BoxCollider2D>().enabled = true;
        _isAttacking = true;
        _model.Speed *= 3;

        yield return new WaitForSeconds(1.1f);

        _model.Speed /= 3;
        _isAttacking = false;
        hitbox.GetComponent<BoxCollider2D>().enabled = false;
    }



    //Check and Change direction
    private void ChangeMoveDirection()
    {
        if (IsDead || _isAttacking) return;

        if (!_isGroundBottom.IsTouchingLayers(LayerMask.GetMask("Ground")) && _changeDirection)
        {
            _changeDirection = false;
            StartCoroutine(ChangeDirectionCorutine());
        }
        else if (_isGroundOpposite.IsTouchingLayers(LayerMask.GetMask("Ground")) && _changeDirection)
        {
            _changeDirection = false;
            StartCoroutine(ChangeDirectionCorutine());
        }
    }

    //Coroutine for Movement
    IEnumerator ChangeDirectionCorutine()
    {
        _model.Speed = -_model.Speed;
        this.transform.localRotation *= Quaternion.Euler(0, 180, 0);
        yield return new WaitForSeconds(0.7f);
        _changeDirection = true;
    }



    public void TakeDamage(int dmg)
    {
        _model.HP -= dmg;
        if (_model.HP <= 0)
        {
            _model.HP = 0;
            _model.Speed = 0;

            IsDead = true;
            Invoke(nameof(DestroyMe), 1.0f);
        }
        else
        {
            IsHurting = true;
            Invoke(nameof(StopHurting), 0.2f);
        }
    }
    private void DestroyMe() { Destroy(gameObject); }
    private void StopHurting() { IsHurting = false; }



    private void Animate()
    {
        if (IsDead) _view.Die();
        else if (IsHurting) _view.TakeDamage();
        else if (_isAttacking) _view.Attack();
        else _view.Walk();
    }
}
