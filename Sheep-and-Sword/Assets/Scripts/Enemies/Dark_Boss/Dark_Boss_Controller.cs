using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Dark_Boss_Controller : MonoBehaviour, IEntityController
{
    // Animations:
    private Dark_Boss_View view;
    public bool IsHurting { get; private set; }
    public bool IsDead { get; private set; }

    // Movement:
    private Dark_Boss_Model model;
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
    private bool isAttacking = false;
    private bool canUseAttack = true;
    private bool canDash = true;
    private bool isDashing = false;
    private int AttackNumber;
    [SerializeField]
    private float DashSpeed = 3;
    [SerializeField]
    private float AttackSpeed = 3;
    [SerializeField]
    private int hpToAdd = 5;

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
        view = GetComponent<Dark_Boss_View>();
        model = GetComponent<Dark_Boss_Model>();
        rd2D = GetComponent<Rigidbody2D>();
        actionSounds = gameObject.GetComponent<SoundController>();
        movementAudioSource = gameObject.GetComponents<AudioSource>()[1];
        enemyHealthBar = GameObject.Find("UI").transform.Find("EnemyHealthBar").gameObject;
        enemyHealthBarFill = GameObject.Find("UI").transform.Find("EnemyHealthBar").transform.Find("EnemyHealthBar_Fill").gameObject.GetComponent<Image>();
    }
    
    void Start() { changeDirection = true; }

    private void FixedUpdate()
    {
        // Change dark_boss's position:
        rd2D.MovePosition(rd2D.position + new Vector2(model.Speed, 0) * Time.fixedDeltaTime);

        // Check if there is a wall or player in front of dark_boss:
        ChangeMoveDirection();
    }

    private void Update()
    {
        Animate();
        if (inRange) CheckAttack();
    }




    private void CheckAttack()
    {
        if (!isAttacking && canUseAttack)
        {
            // Make a random attack:
            AttackNumber = Random.Range(0, 3);
            AttackStart();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Check if living player showed up in front of dark_boss:
        if (collider.gameObject.CompareTag("Player") && !collider.gameObject.GetComponentInParent<IEntityController>().IsDead)
        {
            inRange = true;
            target = collider.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if player is no longer in front of dark_boss:
        if (collision.gameObject.CompareTag("Player"))
        {
            inRange = false;
            target = null;
        }
    }



    private void AttackStart()
    {
        // Update states not to attack constantly:
        isAttacking = true;
        canUseAttack = false;
        Invoke(nameof(CanUseAttack), 5f);

        if (AttackNumber != 2)
        {
            // Make sounds for attacking:
            Invoke(nameof(SoundAttack), 0.1f);
            Invoke(nameof(SoundAttack), 0.3f);
            Invoke(nameof(SoundAttack), 0.6f);

            // Update state:
            Invoke(nameof(AttackStop), 0.65f);

            // Enable hitbox to make it triggerable so player's health will be decreased after hit:
            hitbox.GetComponent<BoxCollider2D>().enabled = true;

            // Let the dark_boss move faster:
            model.Speed *= AttackSpeed;
        }
        else
        {
            // If attackNumber is 2 and dark_boss HP is at level max - 20, make a heal:
            if (model.HP < model.MaxHP - 20)
                Heal();
            // Otherwise make an attack that has number 1:
            else
            {
                AttackNumber = 1;

                // Make sounds for attacking:
                Invoke(nameof(SoundAttack), 0.1f);
                Invoke(nameof(SoundAttack), 0.3f);
                Invoke(nameof(SoundAttack), 0.6f);

                // Update state:
                Invoke(nameof(AttackStop), 0.65f);

                // Enable hitbox to make them triggerable so player's health will be decreased after hit:
                hitbox.GetComponent<BoxCollider2D>().enabled = true;

                // Let the dark_boss move faster:
                model.Speed *= AttackSpeed;
            }
        }
        
    }

    private void SoundAttack() { actionSounds.PlaySound(3); }

    private void AttackStop()
    {
        // Revert enemy's speed:
        model.Speed /= AttackSpeed;

        // Update state (eg. for animations):
        isAttacking = false;

        // Disable hitbox:
        hitbox.GetComponent<BoxCollider2D>().enabled = false;
    }

    private void CanUseAttack() { canUseAttack = true; }



    private void Heal()
    {
        // Make a "heal" sound:
        actionSounds.PlaySound(0);

        // Freeze movement:
        rd2D.constraints = RigidbodyConstraints2D.FreezeAll;

        // Update health points:
        model.HP += hpToAdd;

        // Update states:
        Invoke(nameof(StopHealing), 1.1f);

        // Update health bar:
        enemyHealthBar.SetActive(true);
        enemyHealthBarFill.fillAmount = (float)ReturnCurrentHP() / ReturnMaxHP();
    }

    private void StopHealing()
    {
        isAttacking = false;
        CanUseAttack();
        rd2D.constraints = RigidbodyConstraints2D.None;
    }



    // Checking if there is a need to turn around:
    private void ChangeMoveDirection(bool behind = false)
    {
        // If dark_boss is dead, do nothing:
        if (IsDead) return;

        // If player attacked from behind, turn around:
        if (behind)
        {
            changeDirection = false;
            StartCoroutine(ChangeDirectionCorutine());
        }

        // If there is no ground in front of dark_boss, turn around:
        if (!isGroundBottom.IsTouchingLayers(LayerMask.GetMask("Ground")) && changeDirection)
        {
            changeDirection = false;
            StartCoroutine(ChangeDirectionCorutine());
        }

        // If there is a wall in front of dark_boss, turn around:
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
        // If dark_boss is dead, do nothing:
        if (IsDead) return;

        // Don't make a sword sound if hurting:
        CancelInvoke(nameof(SoundAttack));

        // Check if player is behind the dark_boss and turn around:
        var p = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 toTarget = (p.position - transform.position).normalized;
        if (Vector3.Dot(toTarget, transform.forward) < 0)
            ChangeMoveDirection(true);

        if (!canDash)
        {
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

                // Remove dark_boss from the map:
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
        else Dash();
    }

    private void DestroyMe() { Destroy(gameObject); }

    private void StopHurting() { IsHurting = false; }

    
    
    private void Dash()
    {
        // Make a Dash sound:
        if (!movementAudioSource.isPlaying) movementAudioSource.Play();

        // Update states (eg. for animations):
        canDash = false;
        isDashing = true;

        // Don't attack anyone:
        gameObject.layer = 30;
        gameObject.GetComponentInChildren<HitBoxController>().damage = 0;

        // Update speed (move faster when attacking):
        model.Speed *= DashSpeed;

        // Revert everything:
        Invoke(nameof(StopDashing), 0.3f);
        
        // Don't dash constantly:
        Invoke(nameof(CanDash), 5f);
    }

    private void StopDashing()
    {
        // Stop making Dash sound:
        if (movementAudioSource.isPlaying) movementAudioSource.Stop();

        // Update states, speed and possibility to attack:
        isDashing = false;
        gameObject.layer = 0;
        gameObject.GetComponentInChildren<HitBoxController>().damage = 5;
        model.Speed /= DashSpeed;
    }

    private void CanDash() { canDash = true; }



    private void Animate()
    {
        if (IsDead) view.Die();
        else if (IsHurting) view.Hurt();
        else if (isDashing) view.Dash();
        else if (isAttacking && !canUseAttack)
        {
            if (AttackNumber == 0) view.Attack1();
            else if (AttackNumber == 1) view.Attack2();
            else if (AttackNumber == 2) view.Heal();
        }
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



    // Get dark_boss's health points' values:
    public int ReturnCurrentHP() { return model.HP; }
    public int ReturnMaxHP() { return model.MaxHP; }
}
