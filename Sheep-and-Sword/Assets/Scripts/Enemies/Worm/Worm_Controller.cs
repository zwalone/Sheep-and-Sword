﻿using System.Collections;
using UnityEngine;

public class Worm_Controller : MonoBehaviour, IEntityController
{
    // Animations:
    private Worm_View view;
    public bool IsHurting { get; private set; }
    public bool IsDead { get; private set; }

    // Movement:
    private Worm_Model model;
    private Rigidbody2D rd2D;
    [SerializeField]
    private CircleCollider2D isGroundBottom;
    [SerializeField]
    private CircleCollider2D isGroundOpposite;
    [SerializeField]
    private bool changeDirection;

    // Player tracking:
    public Transform rayCast;
    public LayerMask rayCastMask;
    public float rayCastLength;
    public float attackDistance;
    private GameObject target;
    private bool inRange;

    // Combat:
    [SerializeField]
    private GameObject hitbox;
    private bool isAttacking;
    [SerializeField]
    private float AttackSpeed = 3;

    // Sounds:
    private SoundController actionSounds;

    // Particles:
    public GameObject particles;
    public Vector2 particleDeltaPosition;



    private void Awake()
    {
        view = GetComponent<Worm_View>();
        model = GetComponent<Worm_Model>();
        rd2D = GetComponent<Rigidbody2D>();
        actionSounds = gameObject.GetComponent<SoundController>();
    }

    void Start() { changeDirection = true; }

    private void FixedUpdate()
    {
        // Change worm's position:
        rd2D.MovePosition(rd2D.position + new Vector2(model.Speed, 0) * Time.fixedDeltaTime);

        // Check if there is a wall or player in front of worm:
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
        // Check if living player showed up in front of worm:
        if (collider.gameObject.CompareTag("Player") && !collider.gameObject.GetComponentInParent<IEntityController>().IsDead)
        {
            inRange = true;
            target = collider.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if player is no longer in front of worm:
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
            // Enable hitbox to make it triggerable so player's health will be decreased after hit:
            hitbox.GetComponent<BoxCollider2D>().enabled = true;

            // Update state (eg. for animations):
            isAttacking = true;

            // Let the worm move faster:
            model.Speed *= AttackSpeed;

            // Make a sound of attacking:
            Invoke(nameof(SoundAttack), 0.1f);

            // Revert everything:
            yield return new WaitForSeconds(0.55f);
            model.Speed /= AttackSpeed;
            isAttacking = false;
            hitbox.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    private void SoundAttack() { actionSounds.PlaySound(0); }



    // Checking if there is a need to turn around:
    private void ChangeMoveDirection(bool behind = false)
    {
        // If worm is dead, do nothing:
        if (IsDead) return;

        // If player attacked from behind, turn around:
        if (behind)
        {
            changeDirection = false;
            StartCoroutine(ChangeDirectionCorutine());
        }

        // If there is no ground in front of worm, turn around:
        if (!isGroundBottom.IsTouchingLayers(LayerMask.GetMask("Ground")) && changeDirection)
        {
            changeDirection = false;
            StartCoroutine(ChangeDirectionCorutine());
        }

        // If there is a wall in front of worm, turn around:
        else if (isGroundOpposite.IsTouchingLayers(LayerMask.GetMask("Ground")) && changeDirection)
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
        // If worm is dead, do nothing:
        if (IsDead) return;

        // Don't make an attack sound if hurting:
        CancelInvoke(nameof(SoundAttack));

        // Check if player is behind the worm and turn around:
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

            // Remove worm from the map:
            Invoke(nameof(DestroyMe), 1.0f);
        }
        else
        {
            // Make a "hurt" sound:
            actionSounds.PlaySound(1);

            // Update state (for animations):
            IsHurting = true;
            Invoke(nameof(StopHurting), 0.2f);
        }
    }

    private void DestroyMe() { Destroy(gameObject); }

    private void StopHurting() { IsHurting = false; }



    private void Animate()
    {
        if (IsDead) view.Die();
        else if (IsHurting) view.Hurt();
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



    // Get worm's health points' values:
    public int ReturnCurrentHP() { return model.HP; }
    public int ReturnMaxHP() { return model.MaxHP; }
}
