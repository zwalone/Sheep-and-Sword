using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IEntityController
{
    private PlayerModel model; // speed, jump force, health points
    private PlayerView view;   // animations

    private Rigidbody2D rigbody;          // for movement
    private CapsuleCollider2D capscol;    // for crouching
    private Transform playerGraphics;     // for displaying

    // Player's children with parameters:
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

    public bool IsHurting { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsSliding { get; private set;  } // can't be attacked if true

    // UI:
    private Text hpText;
    private Text gameoverText;
    private Button restartButton;
    private Button returnButton;


    private void Awake()
    {
        playerGraphics = transform.Find("Graphics");
        view = playerGraphics.GetComponent<PlayerView>();

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

        hpText = GameObject.Find("HealthPointsText").GetComponent<Text>();
        gameoverText = GameObject.Find("GameOverText").GetComponent<Text>();
        restartButton = GameObject.Find("RestartGameButton").GetComponent<Button>();
        returnButton = GameObject.Find("GoToMenuButton").GetComponent<Button>();
        gameoverText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        returnButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        CheckTheGround();
        CheckTheWall();
        CheckTheCeiling();
        //CheckTheSlope();

        Jump();
        Crouch();
        FastFall();
        Move();
        Attack();
        Animate();
    }


    private void CheckTheGround()
    {
        // Checking if player is on the ground:
        Collider2D collider = Physics2D.OverlapCircle(groundChecker.position, groundCheckerRadius, groundLayer);
        isGrounded = (collider != null);
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
    }

    private void CheckTheCeiling()
    {
        // Checking if there is a ceiling above the player:
        Collider2D collider = Physics2D.OverlapCircle(ceilingChecker.position, ceilingCheckerRadius, groundLayer);
        isCeilinged = (collider != null);
    }



    private void Move()
    {
        //Check if you can go:
        Vector2 slopeCheckerPosition = transform.position - new Vector3(0.0f, capscol.size.y / 2, 0.0f);
        RaycastHit2D slopeHitRight = Physics2D.Raycast(slopeCheckerPosition,
            transform.right, slopeCheckerRadius, groundLayer);
        RaycastHit2D slopeHitLeft = Physics2D.Raycast(slopeCheckerPosition,
            -transform.right, slopeCheckerRadius, groundLayer);


        // Run (horizontal movement) + avoid auto-sliding down on slopes:
        float xMove = Input.GetAxisRaw("Horizontal");
        if (xMove != 0)
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
                    rigbody.velocity = new Vector2(rigbody.velocity.x, -model.JumpForce);
                else if (!slopeHitLeft && !slopeHitRight) // running up (at the end of the slope)
                    rigbody.velocity = new Vector2(rigbody.velocity.x, 1.0f);
            }
        }
        // Stopping on the slope => freezing whole movement:
        else if (Input.GetAxisRaw("Vertical") == 0 && isGrounded && isWalled == 0 &&
            ((slopeHitLeft && !view.LookRight) || (slopeHitRight && view.LookRight)))
             rigbody.constraints = RigidbodyConstraints2D.FreezePositionX
                                 | RigidbodyConstraints2D.FreezePositionY
                                 | RigidbodyConstraints2D.FreezeRotation;
        // Not moving => freezing on the X:
        else rigbody.constraints = RigidbodyConstraints2D.FreezePositionX
                                 | RigidbodyConstraints2D.FreezeRotation;


        // Climb (vertical movement) + avoid auto-sliding down on walls:
        if (isWalled != 0 && !isCeilinged)
        {
            AttackStop(); // hide sword, stop sliding

            float yMove = Input.GetAxisRaw("Vertical");
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
        if (isGrounded || isWalled != 0) canSomerSault = true;

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            // Restart all attacks:
            attackViewNumber = -1;
            CancelInvoke(nameof(AttackStart));
            AttackStop();

            if (isGrounded || isWalled != 0) // standard jump (from ground or wall)
                rigbody.velocity = new Vector2(rigbody.velocity.x, model.JumpForce);
            else SomerSault();               // somersault only while falling
        }
    }

    private void SomerSault()
    {
        if (canSomerSault == true)
        {
            isSomerSaulting = true;
            rigbody.velocity = new Vector2(rigbody.velocity.x, model.JumpForce);
            Invoke(nameof(SomerSaultCompleted), animationLength * 2);
            canSomerSault = false;
        }
    }

    private void SomerSaultCompleted() { isSomerSaulting = false; }

    private void FastFall()
    {
        // after clicking DownArrow (or S), player goes down faster
        if (!isGrounded) 
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                rigbody.velocity = new Vector2(0, -model.JumpForce / 1.5f);
    }



    private void Crouch()
    {
        if (isGrounded)
        {
            // "Or isCeilinged" helps in situations when there is still 
            // a ceiling above player (but user stopped holding button):
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) || isCeilinged)
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
        if (Input.GetAxisRaw("Horizontal") != 0 && !isCrouched && isWalled == 0 && !isAttacking && !IsHurting)
        {
            IsSliding = true;
            AttackStart(); // hit just at the beginning of the animation
            Invoke(nameof(AttackStop), animationLength * 2.0f);  // this animation is two times longer than normal attack
        }
    }



    private void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isWalled == 0 && !isAttacking && !IsHurting)
        {
            isAttacking = true;
            attackViewNumber = (attackViewNumber + 1) % 3;   // there are 3 types of attack

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



    private void Animate()
    {
        // Flip:
        if (view.LookRight == true && Input.GetAxisRaw("Horizontal") < 0)
        {
            playerGraphics.GetComponent<SpriteRenderer>().flipX = true;
            view.LookRight = false;
        }
        if (view.LookRight == false && Input.GetAxisRaw("Horizontal") > 0)
        {
            playerGraphics.GetComponent<SpriteRenderer>().flipX = false;
            view.LookRight = true;
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



    public void TakeDamage(int dmg)
    {
        if (dmg <= 0) return; 

        // Decrease health:
        if (model.HP > 0)
        {
            model.HP -= dmg;
            if (model.HP < 0) model.HP = 0;
            UpdateHPText();
        }

        // Animate:
        if (model.HP > 0)
        {
            IsHurting = true;
            Invoke(nameof(StopHurting), 0.2f); // "Hurt" animation will last for 0.2 seconds
        }
        else
        {
            IsDead = true;
            Invoke(nameof(GameOver), animationLength * 2.1f); // Game freezes after Die animation
        }
    }

    private void StopHurting() {  IsHurting = false; }

    private void GameOver()
    {
        gameoverText.gameObject.SetActive(true);
        Time.timeScale = 0;    // freeze everything

        // Because of the freezing, the animation of these buttons also freezes,
        // so we need to find solution better than freezing:
        restartButton.gameObject.SetActive(true);
        returnButton.gameObject.SetActive(true);
    }

    private void UpdateHPText()
    {
        hpText.text = $"HEALTH POINTS: {model.HP} / {model.MaxHP}";
    }
}