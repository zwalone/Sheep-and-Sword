using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonController : MonoBehaviour
{
    public Transform rayCast;
    public LayerMask rayCastMask;
    public float rayCastLength;
    public float attackDistance;

    private RaycastHit2D hit;
    private GameObject target;
    private float distance;
    private bool _inRange;

    private SkeletonModel _model;
    private SkeletonView _view;

    private List<CircleCollider2D> _checkGroundList;
    private Rigidbody2D _rd2D;

    [SerializeField]
    private bool _changeDirection;
    private bool _isAttacking;

    //To remove after check with player
    public bool getdamage;

    private void Awake()
    {
        _view = this.GetComponent<SkeletonView>();
        _model = this.GetComponent<SkeletonModel>();
        _rd2D = this.GetComponent<Rigidbody2D>();
        _checkGroundList = new List<CircleCollider2D>(this.GetComponentsInChildren<CircleCollider2D>());
    }

    void Start()
    {
        _view.Walk();
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
        if (getdamage) TakeDamage(1);

        if (_inRange)
        {
            if (_model.Speed > 0) hit = Physics2D.Raycast(rayCast.position, Vector2.right, rayCastLength, rayCastMask);
            else hit = Physics2D.Raycast(rayCast.position, Vector2.left, rayCastLength, rayCastMask);

            RaycastDebugger();
        }

        //when player is detected
        if (hit.collider != null)
        {
            CheckAttack();

        }
        else if (hit.collider == null)
        {
            _inRange = false;
        }
    }

    //Attack
    private void CheckAttack()
    {
        distance = Vector2.Distance(transform.position, target.transform.position);
        if (distance <= attackDistance && !_isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    //Onlt for debug
    private void RaycastDebugger()
    {
        if (distance > attackDistance)
        {
            if (_model.Speed > 0) Debug.DrawRay(rayCast.position, Vector2.right * rayCastLength, Color.red);
            else Debug.DrawRay(rayCast.position, Vector2.left * rayCastLength, Color.red);
        }
        else
        {
            if (_model.Speed > 0) Debug.DrawRay(rayCast.position, Vector2.right * rayCastLength, Color.green);
            else Debug.DrawRay(rayCast.position, Vector2.left * rayCastLength, Color.green);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            _inRange = true;
            target = collider.gameObject;
        }
    }


    //Check and Change direction
    private void ChangeMoveDirection()
    {
        foreach (var col in _checkGroundList)
        {
            if (!col.IsTouchingLayers(LayerMask.GetMask("Ground")) && _changeDirection)
            {
                _changeDirection = false;
                StartCoroutine(ChangeDirectionCorutine());
            }
        }
    }

    public void TakeDamage(int dmg)
    {
        _model.HP -= dmg;
        if (_model.HP < 0)
        {
            StartCoroutine(Die());
        }
        else
        {
            StartCoroutine(TakeDamage());
        }
        getdamage = false;
    }

    //Coroutine for Movement
    IEnumerator ChangeDirectionCorutine()
    {
        _model.Speed = -_model.Speed;
        this.transform.localRotation *= Quaternion.Euler(0, 180, 0);
        yield return new WaitForSeconds(0.7f);
        _changeDirection = true;
    }

    IEnumerator Die()
    {
        _model.Speed = 0;
        _view.Die();

        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    //Attack
    IEnumerator Attack()
    {
        _isAttacking = true;
        _view.Attack();
        _model.Speed *= 3;

        yield return new WaitForSeconds(1.1f);

        _model.Speed /= 3;
        _view.Walk();
        _isAttacking = false;
    }

    IEnumerator TakeDamage()
    {
        _view.TakeDamage();
        var prevSpeed = _model.Speed;
        _model.Speed = 0;

        yield return new WaitForSeconds(0.2f);

        _model.Speed = prevSpeed;
        _view.Walk();
    }
}
