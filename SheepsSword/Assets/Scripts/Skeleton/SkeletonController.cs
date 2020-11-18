using System.Collections;
using UnityEngine;

public class SkeletonController : MonoBehaviour, IEntityController
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

    private Rigidbody2D _rd2D;

    [SerializeField]
    private bool _changeDirection;
    private bool _isAttacking;

    [SerializeField]
    private GameObject hitbox;

    [SerializeField]
    private CircleCollider2D _isGroundBottom;

    [SerializeField]
    private CircleCollider2D _isGroundOpposite;


    //Parameters:
    bool isHurting;
    bool isDead;




    private void Awake()
    {
        _view = this.GetComponent<SkeletonView>();
        _model = this.GetComponent<SkeletonModel>();
        _rd2D = this.GetComponent<Rigidbody2D>();
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
        Animate();

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

    //Only for debug
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
        if (collider.gameObject.CompareTag("Player"))
        {
            _inRange = true;
            target = collider.gameObject;
        }
    }


    //Check and Change direction
    private void ChangeMoveDirection()
    {
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

    public void TakeDamage(int dmg)
    {
        _model.HP -= dmg;
        if (_model.HP <= 0)
        {
            _model.HP = 0;
            StartCoroutine(Die());
        }
        else
        {
            StartCoroutine(TakeDamage());
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

    IEnumerator Die()
    {
        _model.Speed = 0;
        isDead = true;

        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    //Attack
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

    IEnumerator TakeDamage()
    {
        isHurting = true;
        var prevSpeed = _model.Speed;
        _model.Speed = 0;

        yield return new WaitForSeconds(0.2f);

        _model.Speed = prevSpeed;
        _view.Walk();
        isHurting = false;
    }



    private void Animate()
    {
        if (isHurting) _view.TakeDamage();
        else if (isDead) _view.Die();
        else if (_isAttacking) _view.Attack();
        else _view.Walk();
    }
}
