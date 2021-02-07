using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IEntityController
{
    // General:
    private GameController gm;
    private PlayerModel model;               // speed, jump force, health points
    private PlayerView view;                 // animations
    
    // Mobiles:
    public Joystick joystick;                // input
    private readonly float maxDeviation = 0.3f;

    // Physics:
    private Rigidbody2D rigbody;          // for movement
    private CapsuleCollider2D capscol;    // for crouching
    private Transform playerGraphics;     // for displaying

    // Checkers (for better movement):
    private Transform groundChecker;    // for jumping
    public float groundCheckerRadius;
    public LayerMask groundLayer;
    private Transform ceilingChecker;   // for crouching
    public float ceilingCheckerRadius;
    private Transform wallCheckerLeft;  // for climbing walls
    private Transform wallCheckerRight;
    public float wallCheckerRadius;
    private GameObject hitPointRight;   // for attacking
    private GameObject hitPointLeft;

    // Other parameters:
    private readonly float slopeCheckerRadius = 0.6f;
    private readonly float colliderReductor = 0.8f;
    private readonly float animationLength = 0.25f;
    private int attackViewNumber = -1;
    private bool canSomerSault;
    private bool isAttacking;
    private bool isSomerSaulting;
    private bool isGrounded;  // contact with the ground 
    private bool isCeilinged; // contact with the ceiling
    private bool isCrouched;
    private int isWalled;     // contact with the wall
    public float checkpointHeightDifference = 0.01f;
    private bool hasJumped;

    // Health/Damage parameters:
    public bool IsHurting { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsSliding { get; private set; } // can't be attacked if true

    // UI:
    private Image playerHealthBar;
    private bool isReading; // can't move if true
    private GameObject jumpButton;
    private GameObject attackButton;
    private GameObject skipButton;

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

    // Falling down:
    private float fallingDownVelocity = 0.0f;
    private bool enabledFastFalling = false;


    private void Awake()
    {
        // Display:
        playerGraphics = transform.Find("Graphics");
        view = playerGraphics.GetComponent<PlayerView>();

        // Movement:
        model = GetComponent<PlayerModel>();
        rigbody = GetComponent<Rigidbody2D>();
        capscol = GetComponent<CapsuleCollider2D>();
        groundChecker = transform.Find("GroundChecker");
        ceilingChecker = transform.Find("CeilingChecker");
        wallCheckerLeft = transform.Find("WallCheckerLeft");
        wallCheckerRight = transform.Find("WallCheckerRight");
        hitPointLeft = transform.Find("HitPointLeft").gameObject;
        hitPointRight = transform.Find("HitPointRight").gameObject;
        hitPointRight.SetActive(false);
        hitPointLeft.SetActive(false);

        // UI:
        playerHealthBar = GameObject.Find("PlayerHealthBar_Fill").GetComponent<Image>();
        UpdatePlayerHealthBar();
        jumpButton = GameObject.Find("JumpButton");
        jumpButton.GetComponent<Button>().onClick.AddListener(() => Jump());
        attackButton = GameObject.Find("AttackButton");
        attackButton.GetComponent<Button>().onClick.AddListener(() => Attack());
        skipButton = GameObject.Find("MobileControls").transform.Find("SkipButton").gameObject;

        // Sounds:
        movementAudioSource = gameObject.GetComponents<AudioSource>()[1];
        actionSounds = gameObject.GetComponent<SoundController>();
    }

    private void Start()
    {
        // Good position:
        gm = GameObject.Find("GameMaster").GetComponent<GameController>();
        transform.position = new Vector2(gm.LastCheckpointPosition.x, 
            gm.LastCheckpointPosition.y - checkpointHeightDifference);
    }

    private void Update()
    {
        CheckTheGround();
        CheckTheWall();
        CheckTheCeiling();

        Crouch();
        FastFall();
        Move();
        Soundimate();
        Animate();

        FixVelocity();
        MeasureFallingDownVelocity();
    }

    private void CheckTheGround()
    {
        // Checking if player is on the ground:
        Collider2D collider = Physics2D.OverlapCircle(groundChecker.position, groundCheckerRadius, groundLayer);

        // Damage after falling down:
        if (isGrounded == false && collider != null && fallingDownVelocity <= -11.0f)
        {
            TakeDamage((Math.Abs((int)fallingDownVelocity) - 10) * 5);
            fallingDownVelocity = 0.0f;
        }

        isGrounded = (collider != null);
        if (isGrounded) canSomerSault = true;
    }

    private void CheckTheWall()
    {
        // Checking if player is on the wall:
        Collider2D collider = Physics2D.OverlapCircle(wallCheckerRight.position, wallCheckerRadius, groundLayer);
        if (collider != null && view.LookRight == true) { isWalled = -1; }
        else
        {
            collider = Physics2D.OverlapCircle(wallCheckerLeft.position, wallCheckerRadius, groundLayer);
            isWalled = (collider != null && view.LookRight == false) ? 1 : 0;
        }

        if (isWalled != 0)
        {
            fallingDownVelocity = 0.0f;
            canSomerSault = true;
        }
    }

    private void CheckTheCeiling()
    {
        // Checking if there is a ceiling above the player:
        Collider2D collider = Physics2D.OverlapCircle(ceilingChecker.position, ceilingCheckerRadius, groundLayer);
        isCeilinged = (collider != null);
    }

    private void FixVelocity()
    {
        if (!isGrounded && rigbody.velocity.y > model.JumpForce)
            rigbody.velocity = new Vector2(rigbody.velocity.x, model.JumpForce);
    }

    private void MeasureFallingDownVelocity()
    {
        if (!isGrounded && isWalled == 0)
            fallingDownVelocity = rigbody.velocity.y;
    }


    private void Move()
    {
        if (IsDead || isReading || Time.timeScale != 1)
        {
            rigbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            return; 
        }

        //Checking slopes:
        RaycastHit2D slopeHitRight = Physics2D.Raycast(groundChecker.position,
            transform.right, slopeCheckerRadius, groundLayer);
        RaycastHit2D slopeHitLeft = Physics2D.Raycast(groundChecker.position,
            -transform.right, slopeCheckerRadius, groundLayer);

        // Run (horizontal movement):
        float xMove;
        if (joystick.Horizontal < -maxDeviation) xMove = -1;
        else if (joystick.Horizontal > maxDeviation) xMove = 1;
        else xMove = 0;

        if (xMove != 0 && !IsDead)
        {
            rigbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            rigbody.velocity = new Vector2(xMove * model.Speed, rigbody.velocity.y);

            // Running on the slopes:
            RaycastHit2D hit = Physics2D.Raycast(groundChecker.position, Vector2.down,
                groundCheckerRadius, groundLayer);
            if (isGrounded && !hasJumped &&
                hit.collider != null && Mathf.Abs(hit.normal.x) > 0.1f)
            {
                if (hit.normal.x * xMove > 0) // running down
                    rigbody.velocity = new Vector2(rigbody.velocity.x, -model.JumpForce / 2);
                else if (!slopeHitLeft && !slopeHitRight) // running up (at the end of the slope)
                    rigbody.velocity = new Vector2(rigbody.velocity.x, 1.0f);
            }
        }
        // Stopping on the slope => freezing whole movement:
        else if (!hasJumped && isGrounded && isWalled == 0 && (slopeHitLeft || slopeHitRight))
            rigbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        
        // Not moving => freezing on the X:
        else rigbody.constraints = RigidbodyConstraints2D.FreezePositionX
                                 | RigidbodyConstraints2D.FreezeRotation;


        // Climb (vertical movement) + avoid auto-sliding down on walls:
        if (isWalled != 0 && !isCeilinged && !IsDead)
        {
            AttackStop(); // hide sword, stop sliding

            float yMove;
            if (joystick.Vertical < -maxDeviation) yMove = -1;
            else if (joystick.Vertical > maxDeviation) yMove = 1;
            else yMove = 0;

            if (yMove != 0)
            {
                rigbody.constraints = RigidbodyConstraints2D.FreezeRotation;
                rigbody.velocity = new Vector2(rigbody.velocity.x, yMove * model.Speed);
            }
            else rigbody.constraints = RigidbodyConstraints2D.FreezePositionY
                                     | RigidbodyConstraints2D.FreezeRotation;
        }
    }



    private void Jump()
    {
        if (IsDead || isReading || Time.timeScale != 1) return;

        // Restart all attacks:
        attackViewNumber = -1;
        CancelInvoke(nameof(AttackStart));
        AttackStop();

        if (isGrounded || isWalled != 0) // jump from ground or wall
        {
            hasJumped = true;
            madeJumpSound = false;
            rigbody.velocity = new Vector2(rigbody.velocity.x, model.JumpForce);
        }
        else                             // somersault only while falling
        {
            hasJumped = true;
            SomerSault();
        }
        Invoke(nameof(StopJumping), 0.1f);
    }
    private void StopJumping() { hasJumped = false; }

    private void SomerSault()
    {
        if (canSomerSault == true)
        {
            madeJumpSound = false;
            isSomerSaulting = true;
            rigbody.velocity = new Vector2(rigbody.velocity.x, model.JumpForce);
            Invoke(nameof(SomerSaultCompleted), animationLength * 2);
            canSomerSault = false;
        }
    }

    private void SomerSaultCompleted() { isSomerSaulting = false; }

    private void FastFall()
    {
        // after tilting joystick down (for the first time), player can immediately go down (eg. after jump):
        if (joystick.Vertical < -maxDeviation)
            if (!isGrounded && !enabledFastFalling && rigbody.velocity.y > -model.JumpForce / 1.5f)
                rigbody.velocity = new Vector2(0, -model.JumpForce / 1.5f);
    }



    private void Crouch()
    {
        if (IsDead || isReading || Time.timeScale != 1) return;

        if (isGrounded)
        {
            // "Or isCeilinged" helps in situations when there is still 
            // a ceiling above player (but user stopped holding button):
            if (joystick.Vertical < -maxDeviation || isCeilinged)
            {
                Slide();

                if (!isCrouched)
                {
                    capscol.offset = new Vector2(capscol.offset.x,
                        capscol.offset.y - capscol.size.y * (1 - colliderReductor) / 2);
                    capscol.size = new Vector2(capscol.size.x, (float)System.Math.Round(capscol.size.y * colliderReductor, 2));
                    isCrouched = true;
                }
                return;
            }

            // Standing after crouching:
            if (isCrouched)
            {
                capscol.size = new Vector2(capscol.size.x, (float)System.Math.Round(capscol.size.y / colliderReductor, 2));
                capscol.offset = new Vector2(capscol.offset.x,
                    capscol.offset.y + capscol.size.y * (1 - colliderReductor) / 2);
                isCrouched = false;
            }
        }
    }

    private void Slide()
    {
        if (Math.Abs(joystick.Horizontal) > maxDeviation && !isCrouched && isWalled == 0 && !isAttacking && !IsHurting)
        {
            IsSliding = true;
            StartCoroutine(ShowSlideParticles());
            AttackStart(); // hit just at the beginning of the animation
            Invoke(nameof(AttackStop), animationLength * 2.0f);  // this animation is two times longer than normal attack
        }
    }



    private void Attack()
    {
        if (IsDead || isReading || Time.timeScale != 1) return;

        if (isWalled == 0 && !isAttacking && !IsHurting)
        {
            isAttacking = true;
            attackViewNumber = (attackViewNumber + 1) % 3;   // there are 3 types of attack

            madeAttackSound = false;
            Invoke(nameof(AttackStart), animationLength / 2.0f); // hit in the half of animation
            Invoke(nameof(AttackStop), animationLength);

            if (!isGrounded) attackViewNumber = -1; // reseting standard attacks (view) while in air
        }
    }

    private void AttackStart()
    {
        if (view.LookRight) hitPointRight.SetActive(true);
        else hitPointLeft.SetActive(true);
    }

    private void AttackStop()
    {
        isAttacking = false;
        hitPointRight.SetActive(false);
        hitPointLeft.SetActive(false);

        IsSliding = false;
    }



    public void TakeDamage(int dmg)
    {
        if (IsDead || isReading) return;

        // Decrease health:
        if (model.HP > 0)
        {
            model.HP -= dmg;
            if (model.HP < 0) model.HP = 0;
            UpdatePlayerHealthBar();
        }

        // Animate:
        StartCoroutine(ShowHitParticles()); // particles
        if (model.HP > 0)
        {
            actionSounds.PlaySound(5); // "Hurt" sound
            IsHurting = true;
            Invoke(nameof(StopHurting), 0.2f); // "Hurt" animation will last for 0.2 seconds
        }
        else
        {
            actionSounds.PlaySound(5); // "Hurt" sound
            IsDead = true;
            Invoke(nameof(GameOver), animationLength * 2.1f); // Game ends after Die animation
        }
    }

    private void StopHurting() { IsHurting = false; }

    private void GameOver()
    {
        gameObject.layer = 31; // DeadPlayer Layer, ignored by enemies
        gm.GameOver();
    }

    public void UpdatePlayerHealthBar()
    {
        playerHealthBar.fillAmount = (float)model.HP / model.MaxHP;
        if (playerHealthBar.fillAmount < 0.25f) playerHealthBar.color = Color.red;
        else if (playerHealthBar.fillAmount < 0.5f) playerHealthBar.color = new Color(1.0f, 0.64f, 0.0f); //orange
        else if (playerHealthBar.fillAmount < 0.75f) playerHealthBar.color = Color.yellow;
        else playerHealthBar.color = Color.green;
    }


    private void Animate()
    {
        if (Time.timeScale != 1) return;

        // Flip:
        if (!IsDead && !isReading)
        {
            if (joystick.Horizontal < -maxDeviation)
            {
                playerGraphics.GetComponent<SpriteRenderer>().flipX = true;
                view.LookRight = false;
            }
            if (joystick.Horizontal > maxDeviation)
            {
                playerGraphics.GetComponent<SpriteRenderer>().flipX = false;
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
        else // in air
        {
            if (isAttacking) view.AirAttack();
            else if (rigbody.velocity.y < 0.5f) view.Fall();
            else if (isSomerSaulting) view.SomerSault();
            else view.Jump();
        }
    }

    private void Soundimate()
    {
        // In pause-menu:
        if (Time.timeScale != 1) movementAudioSource.Stop();

        // Running:
        else if (rigbody.velocity.x != 0 && isGrounded && !IsHurting && !IsSliding && isWalled == 0 && !isCrouched)
        {
            movementAudioSource.clip = movementClips[0];
            if (!movementAudioSource.isPlaying) movementAudioSource.Play();
        }

        // Sliding:
        else if (IsSliding)
        {
            movementAudioSource.clip = movementClips[1];
            if (!movementAudioSource.isPlaying) movementAudioSource.Play();
        }

        // Movement on the wall:
        else if (rigbody.velocity.y != 0.0f && isWalled != 0)
        {
            if (rigbody.velocity.y > 0.0f)
            {
                movementAudioSource.clip = movementClips[2];
                if (!movementAudioSource.isPlaying) movementAudioSource.Play();
            }
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
                if (isAttacking && !madeAttackSound) // attack while crouching
                {
                    actionSounds.PlaySound(0);
                    madeAttackSound = true;
                }
            }
            else // standing
            {
                if (isAttacking && !madeAttackSound) // attacks
                {
                    if (attackViewNumber == 0) actionSounds.PlaySound(0);
                    else if (attackViewNumber == 1) actionSounds.PlaySound(1);
                    else actionSounds.PlaySound(2);
                    madeAttackSound = true;
                }
                else if (!madeJumpSound) // jump from ground
                {
                    actionSounds.PlaySound(4);
                    madeJumpSound = true;
                }
            }
        }
        else // in air
        {
            if (isAttacking && !madeAttackSound) // attack in air
            {
                actionSounds.PlaySound(0);
                madeAttackSound = true;
            }
            else if (isSomerSaulting && !madeJumpSound) // somersault
            {
                actionSounds.PlaySound(3);
                madeJumpSound = true;
            }
        }
    }

    private IEnumerator ShowHitParticles()
    {
        GameObject particles = Instantiate(hitParticles,
            new Vector2(transform.position.x - hitParticlesDeltaPosition.x,
            transform.position.y - hitParticlesDeltaPosition.y), Quaternion.identity);
        particles.GetComponent<ParticleSystem>().Play();
        float ttl = particles.gameObject.GetComponent<ParticleSystem>().main.duration;
        yield return new WaitForSeconds(ttl);
        Destroy(particles);
    }

    private IEnumerator ShowSlideParticles()
    {
        Vector3 rot = slideParticles.transform.eulerAngles;
        GameObject particles = Instantiate(slideParticles,
            new Vector2(transform.position.x - slideParticlesDeltaPosition.x, transform.position.y - slideParticlesDeltaPosition.y),
            Quaternion.Euler(rot.x, (view.LookRight) ? rot.y - 90 : rot.y + 90, rot.z));
        particles.GetComponent<ParticleSystem>().Play();

        float ttl = particles.gameObject.GetComponent<ParticleSystem>().main.duration;
        float i = 0;

        while (i < ttl)
        {
            if (!IsSliding) { Destroy(particles); break; }

            particles.transform.position = new Vector2(
                (view.LookRight) ? transform.position.x + slideParticlesDeltaPosition.x : transform.position.x - slideParticlesDeltaPosition.x, 
                transform.position.y - slideParticlesDeltaPosition.y);
            particles.transform.rotation = Quaternion.Euler(rot.x, (view.LookRight) ? rot.y - 90 : rot.y + 90, rot.z);

            i += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        
        if (particles != null) Destroy(particles);
    }



    public int ReturnCurrentHP() { return model.HP;  }
    public int ReturnMaxHP() { return model.MaxHP; }
    public void StartReading() 
    { 
        isReading = true; 
        gameObject.layer = 31; 
        attackButton.SetActive(false); 
        jumpButton.SetActive(false); 
        skipButton.SetActive(true);
    }
    public void StopReading() 
    { 
        isReading = false; 
        gameObject.layer = 9; 
        attackButton.SetActive(true); 
        jumpButton.SetActive(true); 
        skipButton.SetActive(false); 
    }
}