using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclopController : MonoBehaviour, IEntityController
{
    private CyclopModel _model;
    private CyclopView _view;

    private List<CircleCollider2D> _checkGroundList;
    private Rigidbody2D _rd2D;

    private bool _canUseLaser = true;
    private readonly float _laserCooldown = 1.5f;

    //Parameters:
    [SerializeField]
    private bool _changeDirection;
    private bool _isAttacking;
    bool isHurting;
    bool isDead;

    private void Awake()
    {
        _view = this.GetComponent<CyclopView>();
        _model = this.GetComponent<CyclopModel>();
        _rd2D = this.GetComponent<Rigidbody2D>();
        _checkGroundList = new List<CircleCollider2D>(this.GetComponentsInChildren<CircleCollider2D>());
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
        RayCastCheckUpdate();
    }

    private void Update()
    {
        Animate();
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

    //Check Collision With Player by Raycast
    private void RayCastCheckUpdate()
    {
        Vector2 directionRay;
        if (_model.Speed < 0) directionRay = new Vector2(-1, 0);
        else directionRay = new Vector2(1, 0);

        //Add offset to don't hit yourself
        float offset = (_model.Speed < 0 ? -0.4f : 0.4f);
        Vector2 start = new Vector2(this.transform.position.x + offset, this.transform.position.y - .5f);
        RaycastHit2D hit = Physics2D.Raycast(start, directionRay, 5f);

        Debug.DrawRay(start, directionRay, Color.red);

        if (hit.collider) 
        {
            if(hit.collider.CompareTag("Player"))
            {
                if (!_isAttacking)
                {
                    //Debug.Log("Hit Raycast with " + hit.transform.tag);
                    
                    //TODO make a laser
                    StartCoroutine(Attack());
                }
            }
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
        _isAttacking = true;
        float prevSpeed = _model.Speed;

        //Spawn a bullet
        if (_canUseLaser == true)
        {
            _canUseLaser = false;
            if (_model.Speed < 0)
            {
                Instantiate(_model.Laser, this.transform.position, Quaternion.Euler(0, 180, 0));
            }
            else
            {
                Instantiate(_model.Laser, this.transform.position, Quaternion.identity);
            }
            _view.AttackRight();
            Invoke(nameof(CanUseLaser), _laserCooldown);
        }

        //Stop movement during shoot animation 
        _model.Speed = 0;

        yield return new WaitForSeconds(1.5f);

        //Go to previos Movement
        _model.Speed = prevSpeed;
        _isAttacking = false;
    }

    private void CanUseLaser() { _canUseLaser = true; }

    IEnumerator TakeDamage()
    {
        isHurting = true;

        yield return new WaitForSeconds(.3f);

        isHurting = false;
    }

    IEnumerator Die()
    {
        _canUseLaser = false;
        isDead = true;
        _model.Speed = 0;

        yield return new WaitForSeconds(.5f);
        Destroy(gameObject);
    }

    private void Animate()
    {
        if (isHurting) _view.TakeDamage();
        else if (isDead) _view.DieRight();
        else if (_isAttacking) _view.AttackRight();
        else _view.WalkRight();
    }
}
