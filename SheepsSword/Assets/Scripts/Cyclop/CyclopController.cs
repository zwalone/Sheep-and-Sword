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

    // Parameters:
    [SerializeField]
    private bool _changeDirection;
    private bool _isAttacking;

    public bool IsHurting { get; private set; }
    public bool IsDead { get; private set; }



    private void Awake()
    {
        _view = this.GetComponent<CyclopView>();
        _model = this.GetComponent<CyclopModel>();
        _rd2D = this.GetComponent<Rigidbody2D>();
        _checkGroundList = new List<CircleCollider2D>(this.GetComponentsInChildren<CircleCollider2D>());
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
        RayCastCheckUpdate();
    }

    private void Update()
    {
        Animate();
    }




    public void TakeDamage(int dmg)
    {
        //check distance
        var p = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 toTarget = (p.position - transform.position).normalized;
        if (Vector3.Dot(toTarget, transform.forward) < 0)
        {
            Debug.Log("Is behaind");
            ChangeMoveDirection(true);
        }

        _model.HP -= dmg;

        if (_model.HP <= 0)
        {
            _model.HP = 0;
            _model.Speed = 0;
            IsDead = true;
            _canUseLaser = false;
            Invoke(nameof(DestroyMe), 0.5f);
        }
        else
        {
            IsHurting = true;
            Invoke(nameof(StopHurting), 0.3f);
        }
    }

    private void DestroyMe() { Destroy(gameObject); }
    private void StopHurting() { IsHurting = false; }



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
            if(hit.collider.CompareTag("Player") 
                && !hit.collider.gameObject.GetComponentInParent<IEntityController>().IsDead)
            {
                if (!_isAttacking)
                    StartCoroutine(Attack());
            }
        }
    }

    //Check and Change direction
    private void ChangeMoveDirection(bool behind = false)
    {
        if (IsDead) return;

        if (behind)
        {
            _changeDirection = false;
            StartCoroutine(ChangeDirectionCorutine());
        }

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
        if (!IsDead)
        {
            _isAttacking = true;
            float prevSpeed = _model.Speed;

            //Spawn a bullet
            if (_canUseLaser == true)
            {
                _canUseLaser = false;
                if (_model.Speed < 0) Instantiate(_model.Laser, this.transform.position, Quaternion.Euler(0, 180, 0));
                else Instantiate(_model.Laser, this.transform.position, Quaternion.identity);
                Invoke(nameof(CanUseLaser), _laserCooldown);
            }

            //Stop movement during shoot animation 
            _model.Speed = 0;

            yield return new WaitForSeconds(1.5f);

            //Go to previos Movement
            _model.Speed = prevSpeed;
            _isAttacking = false;
        }
    }

    private void CanUseLaser() { _canUseLaser = true; }

    private void Animate()
    {
        if (IsHurting) _view.TakeDamage();
        else if (IsDead) _view.DieRight();
        else if (_isAttacking) _view.AttackRight();
        else _view.WalkRight();
    }
}
