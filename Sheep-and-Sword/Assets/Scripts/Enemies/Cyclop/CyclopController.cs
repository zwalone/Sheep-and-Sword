﻿using System.Collections;
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
    [SerializeField]
    private CircleCollider2D isGroundBottom;
    [SerializeField]
    private CircleCollider2D isGroundOpposite;
    [SerializeField]
    private bool changeDirection;

    // Player tracking:
    private GameObject target;
    private bool inRange;

    // Combat:
    private bool isAttacking;
    private bool canUseLaser = true;
    private readonly float laserCooldown = 1.5f;

    // Preventing multi-hit:
    private bool canHurt = true;
    private readonly float unhurtableCooldown = 0.2f;

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
        actionSounds = gameObject.GetComponent<SoundController>();
    }

    void Start() { changeDirection = true; }

    private void FixedUpdate()
    {
        // Change cyclop's position (if is not attacking):
        if (!inRange && !isAttacking) 
            rd2D.MovePosition(rd2D.position + new Vector2(model.Speed, 0) * Time.fixedDeltaTime);

        // Check if there is a wall or player in front of cyclop:
        ChangeMoveDirection();
    }

    private void Update() 
    {
        Animate();
        if (inRange) CheckAttack();
    }



    private void CheckAttack()
    {
        if (!isAttacking && !IsDead)
            StartCoroutine(Attack());
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Check if living player showed up in front of cyclop:
        if (collider.gameObject.CompareTag("Player") && collider.gameObject.layer != 31)
        {
            inRange = true;
            target = collider.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if player is no longer in front of cyclop:
        if (collision.gameObject.CompareTag("Player"))
        {
            inRange = false;
            target = null;
        }
    }



    IEnumerator Attack()
    {
        if (!IsDead)
        {
            isAttacking = true;

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

            // Wait a moment before stopping attacking:
            yield return new WaitForSeconds(1.5f);
            isAttacking = false;
        }
    }

    private void CanUseLaser() { canUseLaser = true; }



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
        if (!isGroundBottom.IsTouchingLayers(LayerMask.GetMask("Ground")) && changeDirection)
        {
            changeDirection = false;
            StartCoroutine(ChangeDirectionCorutine());
        }

        // If there is a wall in front of cyclop, turn around:
        else if ((isGroundOpposite.IsTouchingLayers(LayerMask.GetMask("Ground"))
               || isGroundOpposite.IsTouchingLayers(LayerMask.GetMask("NoAccessLine"))) && changeDirection)
        {
            changeDirection = false;
            StartCoroutine(ChangeDirectionCorutine());
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



    public void TakeDamage(int dmg)
    {
        // If cyclop is dead or just received damage, do nothing:
        if (IsDead || !canHurt) return;

        // Check if player is behind the cyclop and turn around:
        var p = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 toTarget = (p.position - transform.position).normalized;
        if (Vector3.Dot(toTarget, transform.forward) < 0)
            ChangeMoveDirection(true);

        // Decrease health points:
        model.HP -= dmg;

        // Show hurt particles:
        StartCoroutine(ShowParticles());

        // Update canHurt state:
        canHurt = false;
        Invoke(nameof(MakeHurtable), unhurtableCooldown);

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

    private void MakeHurtable() { canHurt = true; }



    private void Animate()
    {
        if (IsHurting) view.Hurt();
        else if (IsDead) view.Die();
        else if (inRange || isAttacking) view.Attack();
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
