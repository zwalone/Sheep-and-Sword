using System.Collections;
using UnityEngine;

public class MinotaurController : MonoBehaviour, IEntityController
{
    public Transform rayCast;
    public LayerMask rayCastMask;
    public float rayCastLength;
    public float attackDistance;

    private RaycastHit2D hit;
    private GameObject target;
    private float distance;
    private bool _inRange;

    private MinotaurModel _model;
    private MinotaurView _view;

    [SerializeField]
    private CircleCollider2D _isGroundBottom;

    [SerializeField]
    private CircleCollider2D _isGroundOpposite;

    private Rigidbody2D _rd2D;

    private bool _changeDirection;
    private bool _isAttacking;

    [SerializeField]
    private GameObject hitbox;
    

    private void Awake()
    {
        _view = this.GetComponent<MinotaurView>();
        _model = this.GetComponent<MinotaurModel>();
        _rd2D = this.GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        _view.WalkRight();
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

        if (_inRange)
        {
            if(_model.Speed > 0) hit = Physics2D.Raycast(rayCast.position, Vector2.right, rayCastLength, rayCastMask);
            else hit = Physics2D.Raycast(rayCast.position, Vector2.left, rayCastLength, rayCastMask);

            RaycastDebugger();
        }

        //when player is detected
        if(hit.collider != null)
        {
            CheckAttack();

        }else if(hit.collider == null)
        {
            _inRange = false;
        }
    }
    
    //Attack
    private void CheckAttack()
    {
        distance = Vector2.Distance(transform.position, target.transform.position);
        if(distance <= attackDistance && !_isAttacking )
        {
            StartCoroutine(Attack());
        }
    }

    //Onlt for debug
    private void RaycastDebugger()
    {
        if(distance > attackDistance)
        {
            if(_model.Speed > 0) Debug.DrawRay(rayCast.position, Vector2.right * rayCastLength, Color.red);
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
        if(collider.gameObject.CompareTag("Player"))
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
        }else if(_isGroundOpposite.IsTouchingLayers(LayerMask.GetMask("Ground")) && _changeDirection)
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

    IEnumerator Die()
    {
        _model.Speed = 0;
        _view.DieRight();

        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    //Coroutine for Movement
    IEnumerator ChangeDirectionCorutine()
    {
        _model.Speed = -_model.Speed;
        this.transform.localRotation *= Quaternion.Euler(0, 180, 0);
        yield return new WaitForSeconds(0.7f);
        _changeDirection = true;
    }

    IEnumerator Attack()
    {
        hitbox.GetComponent<BoxCollider2D>().enabled = true;
        _isAttacking = true;
        _view.AttackRight();

        _model.Speed *= 6;

        yield return new WaitForSeconds(1.1f);

        _model.Speed /= 6;
        _view.WalkRight();
        _isAttacking = false;
        hitbox.GetComponent<BoxCollider2D>().enabled = true;
    }

    IEnumerator TakeDamage()
    {
        _view.GetDamage();

        yield return new WaitForSeconds(0.3f);

       _view.WalkRight();
    }
}
