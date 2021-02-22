using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclopController : MonoBehaviour, IEntityController
{
    // Animations:
    private CyclopView view;
    public bool IsHurting { get; private set; }
    public bool IsDead { get; private set; }

    // Movement:
    private CyclopModel model;
    private Rigidbody2D rd2D;
    private List<CircleCollider2D> checkGroundList;

    [SerializeField]
    private bool changeDirection;

    // Combat:
    private bool isAttacking;
    private bool canUseLaser = true;
    private readonly float laserCooldown = 1.5f;

    // Sounds:
    private SoundController actionSounds;

    // Particles:
    public GameObject particles;
    public Vector2 particleDeltaPosition;



    private void Awake()
    {
        view = GetComponent<CyclopView>();
        model = GetComponent<CyclopModel>();
        rd2D = GetComponent<Rigidbody2D>();
        checkGroundList = new List<CircleCollider2D>(GetComponentsInChildren<CircleCollider2D>());
        actionSounds = gameObject.GetComponent<SoundController>();
    }

    void Start() { changeDirection = true; }

    private void FixedUpdate()
    {
        // Change cyclop's position:
        rd2D.MovePosition(rd2D.position + new Vector2(model.Speed, 0) * Time.fixedDeltaTime);

        // Check if there is a wall or player in front of cyclop:
        ChangeMoveDirection();
        RayCastCheckUpdate();
    }

    private void Update() { Animate(); }



    public void TakeDamage(int dmg)
    {
        // If cyclop is dead, do nothing:
        if (IsDead) return;

        // Check if player is behind the cyclop and turn around:
        var p = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 toTarget = (p.position - transform.position).normalized;
        if (Vector3.Dot(toTarget, transform.forward) < 0)
            ChangeMoveDirection(true);

        // Decrease health points:
        model.HP -= dmg;

        // Show hurt particles:
        StartCoroutine(ShowParticles());

        // Hurt or die:
        if (model.HP <= 0)
        {
            // Make a "die" sound:
            actionSounds.PlaySound(2);

            // Prevent situations of having less than 0 health points:
            model.HP = 0;

            // Stop the movement:
            model.Speed = 0;

            // Update states (eg. for animations):
            IsDead = true;
            canUseLaser = false;

            // Remove cyclop from the map:
            Invoke(nameof(DestroyMe), 0.5f);
        }
        else
        {
            // Make a "hurt" sound:
            actionSounds.PlaySound(1);

            // Update state (for animations):
            IsHurting = true;
            Invoke(nameof(StopHurting), 0.3f);
        }
    }

    private void DestroyMe() { Destroy(gameObject); }

    private void StopHurting() { IsHurting = false; }



    // Checking collision with Player by Raycast:
    private void RayCastCheckUpdate()
    {
        // Prepare a ray in specific direction:
        Vector2 directionRay;
        if (model.Speed < 0) directionRay = new Vector2(-1, 0);
        else directionRay = new Vector2(1, 0);

        // Add offset not to hit yourself:
        float offset = (model.Speed < 0 ? -0.4f : 0.4f);
        Vector2 start = new Vector2(transform.position.x + offset, transform.position.y - .5f);

        // Show ray (in debug mode):
        Debug.DrawRay(start, directionRay, Color.red);

        // If found living player in front of cyclop, start attacking him:
        RaycastHit2D hit = Physics2D.Raycast(start, directionRay, 5f);
        if (hit.collider)
            if (hit.collider.CompareTag("Player") && !hit.collider.gameObject.GetComponentInParent<IEntityController>().IsDead)
                if (!isAttacking)
                    StartCoroutine(Attack());
    }

    // Checking if there is a need to turn around:
    private void ChangeMoveDirection(bool behind = false)
    {
        // If cyclop is dead, do nothing:
        if (IsDead) return;

        // If player attacked from behind, turn around:
        if (behind)
        {
            changeDirection = false;
            StartCoroutine(ChangeDirectionCorutine());
        }

        // If there is no ground in front of cyclop, turn around:
        foreach (var col in checkGroundList)
        {
            if (!col.IsTouchingLayers(LayerMask.GetMask("Ground")) && changeDirection)
            {
                changeDirection = false;
                StartCoroutine(ChangeDirectionCorutine());
            }
        }
    }

    // Turning around, changing direction:
    IEnumerator ChangeDirectionCorutine()
    {
        model.Speed = -model.Speed;
        transform.localRotation *= Quaternion.Euler(0, 180, 0);
        yield return new WaitForSeconds(0.7f);
        changeDirection = true;
    }



    IEnumerator Attack()
    {
        if (!IsDead)
        {
            isAttacking = true;
            float prevSpeed = model.Speed;

            if (canUseLaser == true)
            {
                // Update state:
                canUseLaser = false;

                // Spawn a bullet:
                if (model.Speed < 0) Instantiate(model.Laser, transform.position, Quaternion.Euler(0, 180, 0));
                else Instantiate(model.Laser, transform.position, Quaternion.identity);

                // Make a sound:
                actionSounds.PlaySound(0);

                // Update state after some time (prevent attacking constantly):
                Invoke(nameof(CanUseLaser), laserCooldown);
            }

            // Stop movement during shoot animation:
            model.Speed = 0;
            yield return new WaitForSeconds(1.5f);

            // Go to previous movement:
            model.Speed = prevSpeed;
            isAttacking = false;
        }
    }

    private void CanUseLaser() { canUseLaser = true; }



    private void Animate()
    {
        if (IsHurting) view.Hurt();
        else if (IsDead) view.Die();
        else if (isAttacking) view.Attack();
        else view.Walk();
    }

    private IEnumerator ShowParticles()
    {
        // Show hurt particles:
        GameObject par = Instantiate(particles,
            new Vector2(transform.position.x - particleDeltaPosition.x,
            transform.position.y - particleDeltaPosition.y), Quaternion.identity);
        par.GetComponent<ParticleSystem>().Play();

        // Destroy hurt particles after ttl seconds:
        float ttl = par.gameObject.GetComponent<ParticleSystem>().main.duration;
        yield return new WaitForSeconds(ttl);
        Destroy(par);
    }



    // Get cyclop's health points' values:
    public int ReturnCurrentHP() { return model.HP; }
    public int ReturnMaxHP() { return model.MaxHP; }
}
