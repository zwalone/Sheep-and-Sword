using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Dark_Boss_Controller : MonoBehaviour, IEntityController
{
    public Transform rayCast;
    public LayerMask rayCastMask;
    public float rayCastLength;
    public float attackDistance;

    private GameObject target;
    private bool _inRange;

    private Dark_Boss_Model _model;
    private Dark_Boss_View _view;

    private Rigidbody2D _rd2D;

    [SerializeField]
    private GameObject[] hitbox;

    [SerializeField]
    private CircleCollider2D _isGroundBottom;

    [SerializeField]
    private CircleCollider2D _isGroundOpposite;

    [SerializeField]
    private float SpeedDash = 3;

    [SerializeField]
    private float SpeedAttack = 3;

    // Parameters:
    [SerializeField]
    private bool _changeDirection;

    private bool _isAttacking = false;
    private bool _canUseAttack = true;
    private bool _canDash = true;
    private bool _isDash = false;

    public bool IsHurting { get; private set; }
    public bool IsDead { get; private set; }
    private int AttackNumber;

    // Sounds:
    private SoundController actionSounds;
    private AudioSource movementAudioSource;

    // Particles:
    public GameObject particles;
    public Vector2 particleDeltaPosition;


    // Boss health bar:
    private GameObject enemyHealthBar;
    private Image enemyHealthBarFill;


    private void Awake()
    {
        _view = this.GetComponent<Dark_Boss_View>();
        _model = this.GetComponent<Dark_Boss_Model>();
        _rd2D = this.GetComponent<Rigidbody2D>();
        actionSounds = gameObject.GetComponent<SoundController>();
        movementAudioSource = gameObject.GetComponents<AudioSource>()[1];
        enemyHealthBar = GameObject.Find("UI").transform.Find("EnemyHealthBar").gameObject;
        enemyHealthBarFill = GameObject.Find("UI").transform.Find("EnemyHealthBar").transform.Find("EnemyHealthBar_Fill").gameObject.GetComponent<Image>();
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
        if (!_isAttacking && _canUseAttack)
        {
            AttackNumber = Random.Range(0, 3);
            AttackStart();
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

    private void AttackStart()
    {

        _isAttacking = true;
        _canUseAttack = false;

        Invoke(nameof(CanUseAttack), 5f);

        if (AttackNumber != 2)
        {
            Invoke(nameof(SoundAttack), 0.1f);
            Invoke(nameof(SoundAttack), 0.3f);
            Invoke(nameof(SoundAttack), 0.6f);


            Invoke(nameof(AttackStop), 0.65f);

            foreach (var h in hitbox)
            {
                h.GetComponent<BoxCollider2D>().enabled = true;
                //TOChange
                _model.Speed *= SpeedAttack;
            }
        }
        else
        {
            //If have max hp
            if(_model.HP < _model.MaxHP - 20)
            {
                Heal();
            }
            else
            {
                Invoke(nameof(SoundAttack), 0.1f);
                Invoke(nameof(SoundAttack), 0.3f);
                Invoke(nameof(SoundAttack), 0.6f);

                AttackNumber = 1;
                Invoke(nameof(AttackStop), 0.65f);

                foreach (var h in hitbox)
                {
                    h.GetComponent<BoxCollider2D>().enabled = true;
                    //TOChange
                    _model.Speed *= 3;
                }
            }
        }
        
    }
    private void SoundAttack() { actionSounds.PlaySound(3); }

    private void AttackStop()
    {
        _model.Speed /= SpeedAttack;
        _isAttacking = false;
        foreach (var h in hitbox)
        {
            h.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    private void Heal()
    {
        actionSounds.PlaySound(0);
        _rd2D.constraints = RigidbodyConstraints2D.FreezeAll;
        _model.HP += 5;
        Invoke(nameof(IsAttack), 1.1f);

        enemyHealthBar.SetActive(true);
        enemyHealthBarFill.fillAmount = (float)ReturnCurrentHP() / ReturnMaxHP();
    }

    //Check and Change direction
    private void ChangeMoveDirection(bool behind = false)
    {
        if (IsDead || _isAttacking) return;

        if (behind)
        {
            _changeDirection = false;
            StartCoroutine(ChangeDirectionCorutine());
        }

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
        if (IsDead) return;
        CancelInvoke(nameof(SoundAttack));

        //check distance
        var p = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 toTarget = (p.position - transform.position).normalized;
        if (Vector3.Dot(toTarget, transform.forward) < 0)
        {
            ChangeMoveDirection(true);
        }


        if (!_canDash)
        {
            _model.HP -= dmg;
            StartCoroutine(ShowParticles());

            if (_model.HP <= 0)
            {
                actionSounds.PlaySound(2);
                _model.HP = 0;
                _model.Speed = 0;

                IsDead = true;
                Invoke(nameof(DestroyMe), 1.0f);
            }
            else
            {
                actionSounds.PlaySound(1);
                IsHurting = true;
                Invoke(nameof(StopHurting), 0.2f);
            }
        }
        else
        {
            Dash();
        }

    }

    private void Dash()
    {
        if (!movementAudioSource.isPlaying) movementAudioSource.Play();

        _canDash = false;
        _isDash = true;

        gameObject.layer = 30;
        gameObject.GetComponentInChildren<HitBoxController>().damage = 0;

        //Speed
        _model.Speed *= SpeedDash;

        Invoke(nameof(StopDashing), 0.3f);
        //Delay Dash
        Invoke(nameof(CanDash), 5f);
    }
    private void StopDashing()
    {
        if (movementAudioSource.isPlaying) movementAudioSource.Stop();

        _isDash = false;
        gameObject.layer = 0;
        gameObject.GetComponentInChildren<HitBoxController>().damage = 5;
        _model.Speed /= SpeedDash;
    }



    private void DestroyMe() { Destroy(gameObject); }
    private void StopHurting() { IsHurting = false; }
    private void CanUseAttack() { _canUseAttack = true; }
    private void CanDash() { _canDash = true; }
    private void IsAttack() {
        _isAttacking = false;
        CanUseAttack();
        _rd2D.constraints = RigidbodyConstraints2D.None;
    }


    private void Animate()
    {
        if (IsDead) _view.Die();
        else if (IsHurting) _view.TakeDamage();
        else if (_isDash) _view.Dash();
        else if (_isAttacking && !_canUseAttack)
        {
            if (AttackNumber == 0) _view.Attack1();
            else if (AttackNumber == 1) _view.Attack2();
            else if (AttackNumber == 2) _view.Heal();
        }
        else _view.Walk();
    }

    private IEnumerator ShowParticles()
    {
        GameObject firework = Instantiate(particles,
            new Vector2(transform.position.x - particleDeltaPosition.x,
            transform.position.y - particleDeltaPosition.y), Quaternion.identity);
        firework.GetComponent<ParticleSystem>().Play();
        float ttl = firework.gameObject.GetComponent<ParticleSystem>().main.duration;
        yield return new WaitForSeconds(ttl);
        Destroy(firework);
    }


    public int ReturnCurrentHP() { return _model.HP; }
    public int ReturnMaxHP() { return _model.MaxHP; }
}
