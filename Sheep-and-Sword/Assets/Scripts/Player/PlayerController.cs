using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IEntityController
{
    private GameController gm;            

    // Animations:
    private PlayerView view;         
    private readonly float animationLength = 0.25f;
    public bool IsHurting { get; private set; }
    public bool IsDead { get; private set; }

    // Movement:
    private PlayerModel model;
    private Rigidbody2D rigbody;
    private CapsuleCollider2D capscol;
    private readonly float slopeCheckerRadius = 0.6f;
    private readonly float colliderReductor = 0.8f;
    private bool isCrouched;

    // Jumping:
    private bool canSomerSault;
    private bool isSomerSaulting;

    // Touching the ground:
    private Transform groundChecker;
    public float groundCheckerRadius;
    public LayerMask groundLayer;
    private bool isGrounded;

    // Touching the ceiling:
    private Transform ceilingChecker;
    public float ceilingCheckerRadius;
    private bool isCeilinged;

    // Touching the walls:
    private Transform wallChecker;
    public float wallCheckerRadius;
    private int isWalled;

    // Combat:
    [SerializeField]
    private GameObject hitbox;
    private int attackViewNumber = -1;
    private bool isAttacking;
    public bool IsSliding { get; private set; }

    // Preventing multi-hit:
    private bool canHurt = true;
    private readonly float unhurtableCooldown = 0.2f;

    // Falling down:
    private float fallingDownVelocity = 0.0f;
    private readonly bool enabledFastFalling = false;

    // For checkpoints (respawn):
    public float checkpointHeightDifference = 0.01f;

    // UI:
    private Image playerHealthBar;
    private bool isReading; // can't move or be attacked if true

    // Sounds:
    private SoundController actionSounds;
    public List<AudioClip> movementClips;
    private AudioSource movementAudioSource;
    private bool madeAttackSound = true;
    private bool madeJumpSound = true;

    // Particles:
    public GameObject hitParticles;
    public Vector2 hitParticlesDeltaPosition;
    public GameObject slideParticles;
    public Vector2 slideParticlesDeltaPosition;


    private void Awake()
    {
        // Display:
        view = GetComponent<PlayerView>();

        // Movement:
        model = GetComponent<PlayerModel>();
        rigbody = GetComponent<Rigidbody2D>();
        capscol = GetComponent<CapsuleCollider2D>();
        groundChecker = transform.Find("GroundChecker");
        ceilingChecker = transform.Find("CeilingChecker");
        wallChecker = transform.Find("WallChecker");

        // UI:
        playerHealthBar = GameObject.Find("PlayerHealthBar_Fill").GetComponent<Image>();
        UpdatePlayerHealthBar();

        // Sounds:
        movementAudioSource = gameObject.GetComponents<AudioSource>()[1];
        actionSounds = gameObject.GetComponent<SoundController>();
    }

    private void Start()
    {
        // Placing player in position of last reached checkpoint (or first checkpoint):
        gm = GameObject.Find("GameMaster").GetComponent<GameController>();
        transform.position = new Vector2(gm.LastCheckpointPosition.x, 
            gm.LastCheckpointPosition.y - checkpointHeightDifference);
    }

    private void Update()
    {
        CheckTheGround();
        CheckTheWall();
        CheckTheCeiling();

        Jump();
        Crouch();
        FastFall();
        Move();
        Attack();
        Soundimate();
        Animate();

        FixVelocity();
        MeasureFallingDownVelocity();
    }



    private void CheckTheGround()
    {
        // Checking if player is touching the ground:
        Collider2D collider = Physics2D.OverlapCircle(groundChecker.position, groundCheckerRadius, groundLayer);

        // Hard landing situation:
        if (isGrounded == false && collider != null && fallingDownVelocity <= -11.0f)
        {
            TakeDamage((Math.Abs((int)fallingDownVelocity) - 10) * 5);
            fallingDownVelocity = 0.0f;
        }

        // Update isGrounded status:
        isGrounded = (collider != null);
    }

    private void CheckTheWall()
    {
        // Checking if player is touching the wall:
        Collider2D collider = Physics2D.OverlapCircle(wallChecker.position, wallCheckerRadius, groundLayer);

        // Update isWalled status:
        if (collider != null && view.LookRight == true) { isWalled = -1; }
        else
        {
            collider = Physics2D.OverlapCircle(wallChecker.position, wallCheckerRadius, groundLayer);
            isWalled = (collider != null && view.LookRight == false) ? 1 : 0;
        }

        // Update fallingDownVelocity to prevent hard landing:
        if (isWalled != 0) fallingDownVelocity = 0.0f;
    }

    private void CheckTheCeiling()
    {
        // Checking if there is a ceiling above the player:
        Collider2D collider = Physics2D.OverlapCircle(ceilingChecker.position, ceilingCheckerRadius, groundLayer);
        isCeilinged = (collider != null);
    }



    // Prevent situations of getting too high velocity:
    private void FixVelocity()
    {
        if (!isGrounded && rigbody.velocity.y > model.JumpForce)
            rigbody.velocity = new Vector2(rigbody.velocity.x, model.JumpForce);
    }

    // Measure falling down velocity to apply hard landing damage:
    private void MeasureFallingDownVelocity() 
    { 
        if (!isGrounded && isWalled == 0) 
            fallingDownVelocity = rigbody.velocity.y; 
    }



    private void Move()
    {
        // If player is dead, is reading a dialog or is in pause-menu, don't move on X axis:
        if (IsDead || isReading || Time.timeScale != 1)
        {
            rigbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            return; 
        }

        // Checking if player has a contact with the ground on his lower corners:
        RaycastHit2D slopeHitRight = Physics2D.Raycast(groundChecker.position,
            transform.right, slopeCheckerRadius, groundLayer);
        RaycastHit2D slopeHitLeft = Physics2D.Raycast(groundChecker.position,
            -transform.right, slopeCheckerRadius, groundLayer);

        // Run (horizontal movement):
        float xMove = Input.GetAxisRaw("Horizontal");
        if (xMove != 0 && !IsDead)
        {
            rigbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            rigbody.velocity = new Vector2(xMove * model.Speed, rigbody.velocity.y);

            // Running on the slopes:
            RaycastHit2D hit = Physics2D.Raycast(groundChecker.position, Vector2.down,
                groundCheckerRadius, groundLayer);
            if (isGrounded && Input.GetAxisRaw("Vertical") == 0 &&
                hit.collider != null && Mathf.Abs(hit.normal.x) > 0.1f)
            {
                if (hit.normal.x * xMove > 0) // running down
                    rigbody.velocity = new Vector2(rigbody.velocity.x, -model.JumpForce / 2);
                else if (!slopeHitLeft && !slopeHitRight) // running up (at the end of the slope)
                    rigbody.velocity = new Vector2(rigbody.velocity.x, 1.0f);
            }
        }

        // Stopping on the slope => freezing whole movement:
        else if (Input.GetAxisRaw("Vertical") == 0 && isGrounded && isWalled == 0 && (slopeHitLeft || slopeHitRight))
            rigbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        
        // Not moving => freezing on the X:
        else rigbody.constraints = RigidbodyConstraints2D.FreezePositionX
                                 | RigidbodyConstraints2D.FreezeRotation;


        // Climb (vertical movement):
        if (isWalled != 0 && !isCeilinged && !IsDead)
        {
            AttackStop(); // hide sword, stop sliding
            float yMove = Input.GetAxisRaw("Vertical");

            // Change position on Y axis:
            if (yMove != 0)
            {
                rigbody.constraints = RigidbodyConstraints2D.FreezeRotation;
                rigbody.velocity = new Vector2(rigbody.velocity.x, yMove * model.Speed);
            }

            // Avoid auto-sliding down on walls:
            else rigbody.constraints = RigidbodyConstraints2D.FreezePositionY
                                     | RigidbodyConstraints2D.FreezeRotation;
        }
    }



    private void Jump()
    {
        // If player is dead, is reading a dialog or is in pause-menu, do nothing:
        if (IsDead || isReading || Time.timeScale != 1) return;

        // Restart possibility of somersaulting if player is touching the ground or the wall:
        if (isGrounded || isWalled != 0) canSomerSault = true;

        // Instructions which have to be done after player's input:
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            // Restart all attacks:
            attackViewNumber = -1;
            AttackStop();

            if (isGrounded || isWalled != 0) // standard jump (from ground or wall)
            {
                madeJumpSound = false;
                rigbody.velocity = new Vector2(rigbody.velocity.x, model.JumpForce);
            }
            else SomerSault();               // somersault only while falling
        }
    }

    private void SomerSault()
    {
        if (canSomerSault == true)
        {
            // Make a somersault sound:
            madeJumpSound = false;

            // Update player's velocity:
            rigbody.velocity = new Vector2(rigbody.velocity.x, model.JumpForce);

            // Show animation:
            isSomerSaulting = true;
            Invoke(nameof(SomerSaultCompleted), animationLength * 2);

            // Disable possibility of second somersault:
            canSomerSault = false;
        }
    }

    private void SomerSaultCompleted() { isSomerSaulting = false; }

    private void FastFall()
    {
        // Player can go down faster in certain conditions (player's input, not too high velocity, in the air):
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            if (!isGrounded && !enabledFastFalling && rigbody.velocity.y > -model.JumpForce / 1.5f)
                rigbody.velocity = new Vector2(0, -model.JumpForce / 1.5f);
    }



    private void Crouch()
    {
        // If player is dead, is reading a dialog or is in pause-menu, do nothing:
        if (IsDead || isReading || Time.timeScale != 1) return;

        if (isGrounded)
        {
            // Instructions which have to be done after player's input or situation 
            // when there is still a ceiling above player (but user stopped holding button):
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) || isCeilinged)
            {
                // Try to do a slide (if player is running):
                Slide();

                // Reduce collider's size (if it hasn't been reduced yet) and update isCrouched state for animations:
                if (!isCrouched)
                {
                    capscol.offset = new Vector2(capscol.offset.x,
                        capscol.offset.y - capscol.size.y * (1 - colliderReductor) / 2);
                    capscol.size = new Vector2(capscol.size.x, (float)Math.Round(capscol.size.y * colliderReductor, 2));
                    isCrouched = true;
                }
                return;
            }

            // Standing after crouching - revert reducing collider's size and update isCrouched state for animations:
            if (isCrouched)
            {
                capscol.size = new Vector2(capscol.size.x, (float)Math.Round(capscol.size.y / colliderReductor, 2));
                capscol.offset = new Vector2(capscol.offset.x,
                    capscol.offset.y + capscol.size.y * (1 - colliderReductor) / 2);
                isCrouched = false;
            }
        }
    }

    private void Slide()
    {
        // Trying to crouch while running causes in making a slide:
        if (Input.GetAxisRaw("Horizontal") != 0 && !isCrouched && isWalled == 0 && !isAttacking && !IsHurting)
        {
            // Update IsSliding state for animations:
            IsSliding = true;

            // Show small particles behind the player:
            StartCoroutine(ShowSlideParticles());

            // Enable hitbox to hit enemy:
            hitbox.GetComponent<BoxCollider2D>().enabled = true;

            // Revert everything after animation:
            Invoke(nameof(AttackStop), animationLength * 2.0f);  // this animation is two times longer than normal attack
        }
    }



    private void Attack()
    {
        // If player is dead, is reading a dialog or is in pause-menu, do nothing:
        if (IsDead || isReading || Time.timeScale != 1) return;

        // Instructions which have to be done after player's input:
        if (Input.GetKeyDown(KeyCode.Space) && isWalled == 0 && !isAttacking && !IsHurting)
        {
            // Update states for animations:
            isAttacking = true;
            attackViewNumber = (attackViewNumber + 1) % 3;   // there are 3 types of attack

            // Make a sound:
            madeAttackSound = false;

            // Enable hitbox to hit enemy:
            hitbox.GetComponent<BoxCollider2D>().enabled = true;

            // Revert everything after animation:
            Invoke(nameof(AttackStop), animationLength);

            // Reset view of standard attacks while in air:
            if (!isGrounded) attackViewNumber = -1;
        }
    }

    private void AttackStop()
    {
        isAttacking = false;
        IsSliding = false;
        hitbox.GetComponent<BoxCollider2D>().enabled = false;
    }



    public void TakeDamage(int dmg)
    {
        // If player is dead, is reading a dialog or just received damage, do nothing:
        if (IsDead || isReading || !canHurt) return;

        // Decrease health:
        if (model.HP > 0)
        {
            model.HP -= dmg;
            if (model.HP < 0) model.HP = 0;
            UpdatePlayerHealthBar();
        }

        // Show hit particles:
        StartCoroutine(ShowHitParticles());

        // Update canHurt state:
        canHurt = false;
        Invoke(nameof(MakeHurtable), unhurtableCooldown);

        // Make "hurt" sound:
        actionSounds.PlaySound(5);

        // Hurt or Die:
        if (model.HP > 0)
        {
            IsHurting = true;
            Invoke(nameof(StopHurting), 0.2f); // "Hurt" animation will last for 0.2 seconds
        }
        else
        {
            IsDead = true;
            Invoke(nameof(GameOver), animationLength * 2.1f); // Game ends after Die animation
        }
    }

    private void StopHurting() { IsHurting = false; }

    private void MakeHurtable() { canHurt = true; }

    private void GameOver()
    {
        // Change player's layer to DeadPlayer Layer, ignored by enemies:
        gameObject.layer = 31;

        // Show GameOver screen:
        gm.GameOver();
    }

    public void UpdatePlayerHealthBar()
    {
        // Update percent of health bar fill:
        playerHealthBar.fillAmount = (float)model.HP / model.MaxHP;

        // Change color of health bar:
        if (playerHealthBar.fillAmount < 0.25f) playerHealthBar.color = Color.red;
        else if (playerHealthBar.fillAmount < 0.5f) playerHealthBar.color = new Color(1.0f, 0.64f, 0.0f); //orange
        else if (playerHealthBar.fillAmount < 0.75f) playerHealthBar.color = Color.yellow;
        else playerHealthBar.color = Color.green;
    }



    private void Animate()
    {
        // If player is in pause-menu, do nothing:
        if (Time.timeScale != 1) return;

        // Flip sprite depending on current movement:
        if (!IsDead && !isReading)
        {
            if (view.LookRight && Input.GetAxisRaw("Horizontal") < 0)
            {
                transform.localRotation *= Quaternion.Euler(0, 180, 0);
                view.LookRight = false;
            }
            if (!view.LookRight && Input.GetAxisRaw("Horizontal") > 0)
            {
                transform.localRotation *= Quaternion.Euler(0, 180, 0);
                view.LookRight = true;
            }
        }

        // Change animation:
        if (IsHurting) view.Hurt();
        else if (IsDead) view.Die();
        else if (IsSliding) view.Slide();
        else if (isWalled != 0)
        {
            if (rigbody.velocity.y > 0.0f) view.Climb();
            else if (rigbody.velocity.y == 0.0f) view.WallHold();
            else view.WallSlide();
        }
        else if (isGrounded) // on the ground
        {
            if (isCrouched)  // crouching
            {
                if (isAttacking) view.CrouchAttack();
                else if (rigbody.velocity.x != 0) view.CrouchWalk();
                else view.Crouch();
            }
            else // standing
            {
                if (isAttacking) // 3 standard attacks
                {
                    if (attackViewNumber == 0) view.Attack1();
                    else if (attackViewNumber == 1) view.Attack2();
                    else view.Attack3();
                }
                else if (rigbody.velocity.x != 0) view.Run();
                else view.Idle();
            }
        }
        else // in the air
        {
            if (isAttacking) view.AirAttack();
            else if (rigbody.velocity.y < 0.5f) view.Fall();
            else if (isSomerSaulting) view.SomerSault();
            else view.Jump();
        }
    }

    private void Soundimate()
    {
        // If player is in pause-menu, stop the walking sound:
        if (Time.timeScale != 1) movementAudioSource.Stop();

        // Running sound:
        else if (rigbody.velocity.x != 0 && isGrounded && !IsHurting && !IsSliding && isWalled == 0 && !isCrouched)
        {
            movementAudioSource.clip = movementClips[0];
            if (!movementAudioSource.isPlaying) movementAudioSource.Play();
        }

        // Sliding sound:
        else if (IsSliding)
        {
            movementAudioSource.clip = movementClips[1];
            if (!movementAudioSource.isPlaying) movementAudioSource.Play();
        }

        // Movement on the wall:
        else if (rigbody.velocity.y != 0.0f && isWalled != 0)
        {
            // Climbing sound:
            if (rigbody.velocity.y > 0.0f)
            {
                movementAudioSource.clip = movementClips[2];
                if (!movementAudioSource.isPlaying) movementAudioSource.Play();
            }

            // Sliding down on the wall sound:
            else
            {
                movementAudioSource.clip = movementClips[1];
                if (!movementAudioSource.isPlaying) movementAudioSource.Play();
            }
        }

        else movementAudioSource.Stop();


        // Other sound effects:
        if (isReading || IsHurting || IsDead || IsSliding || isWalled != 0 || Time.timeScale != 1) return;
        else if (isGrounded) // on the ground
        {
            if (isCrouched)  // crouching
            {
                // Sounds for attacks while crouching:
                if (isAttacking && !madeAttackSound)
                {
                    actionSounds.PlaySound(0);
                    madeAttackSound = true; // prevent making attack sound twice
                }
            }
            else // standing
            {
                // Sounds for standard attacks on the ground:
                if (isAttacking && !madeAttackSound)
                {
                    if (attackViewNumber == 0) actionSounds.PlaySound(0);
                    else if (attackViewNumber == 1) actionSounds.PlaySound(1);
                    else actionSounds.PlaySound(2);
                    madeAttackSound = true; // prevent making attack sound twice
                }

                // Sounds for jumping from the ground:
                else if (!madeJumpSound && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)))
                {
                    actionSounds.PlaySound(4);
                    madeJumpSound = true; // prevent making jump sound twice
                }
            }
        }
        else // in the air
        {
            // Sounds for attacks in the air:
            if (isAttacking && !madeAttackSound)
            {
                actionSounds.PlaySound(0);
                madeAttackSound = true; // prevent making attack sound twice
            }

            // Sounds for somersault:
            else if (isSomerSaulting && !madeJumpSound) // somersault
            {
                actionSounds.PlaySound(3);
                madeJumpSound = true; // prevent making jump/somersault sound twice
            }
        }
    }

    private IEnumerator ShowHitParticles()
    {
        // Spawn hit particles ("red blood"):
        GameObject particles = Instantiate(hitParticles,
            new Vector2(transform.position.x - hitParticlesDeltaPosition.x,
            transform.position.y - hitParticlesDeltaPosition.y), Quaternion.identity);
        particles.GetComponent<ParticleSystem>().Play();

        // Destroy hit particles after ttl seconds:
        float ttl = particles.gameObject.GetComponent<ParticleSystem>().main.duration;
        yield return new WaitForSeconds(ttl);
        Destroy(particles);
    }

    private IEnumerator ShowSlideParticles()
    {
        // Spawn slide particles:
        Vector3 rot = slideParticles.transform.eulerAngles;
        GameObject particles = Instantiate(slideParticles,
            new Vector2(transform.position.x - slideParticlesDeltaPosition.x, transform.position.y - slideParticlesDeltaPosition.y),
            Quaternion.Euler(rot.x, (view.LookRight) ? rot.y - 90 : rot.y + 90, rot.z));
        particles.GetComponent<ParticleSystem>().Play();

        // Destroy slide particles after ttl seconds:
        float ttl = particles.gameObject.GetComponent<ParticleSystem>().main.duration;
        float i = 0;

        // Let the particles follow the player:
        while (i < ttl)
        {
            // Stop following player and destroy particles if he isn't sliding anymore:
            if (!IsSliding) { Destroy(particles); break; }

            // Change position and rotation of particles:
            particles.transform.position = new Vector2(
                (view.LookRight) ? transform.position.x + slideParticlesDeltaPosition.x : transform.position.x - slideParticlesDeltaPosition.x, 
                transform.position.y - slideParticlesDeltaPosition.y);
            particles.transform.rotation = Quaternion.Euler(rot.x, (view.LookRight) ? rot.y - 90 : rot.y + 90, rot.z);

            i += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        
        if (particles != null) Destroy(particles);
    }



    // Get player's health points' values:
    public int ReturnCurrentHP() { return model.HP;  }
    public int ReturnMaxHP() { return model.MaxHP; }



    // Update player's states if he is in Dialog Point:
    public void StartReading() { isReading = true; gameObject.layer = 31; }
    public void StopReading() { isReading = false; gameObject.layer = 9; }
}
