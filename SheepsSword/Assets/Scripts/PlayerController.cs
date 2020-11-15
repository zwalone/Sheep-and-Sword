// TO DO: 
// 1. FIX MOVEMENT (SLOPES) AND HUGE PROBLEM: PLAYER'S POSITION'S VALUE DECREASES ON Y AXIS (ON SLOPES AND WALLS)
// 2. ADD DASH (ARROW_DOWN ON SLOPES?)

using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
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

    private Transform wallCheckerLeft;  // for walljumping
    private Transform wallCheckerRight;
    public float wallCheckerRadius;

    private GameObject hitPointRight;   // for attacking
    private GameObject hitPointLeft;

    // Parameters:
    private readonly float animationLength = 0.25f;
    private int  attackViewNumber = -1;
    private bool canSomerSault = false;
    private bool isAttacking = false;
    private bool isSomerSaulting = false;
    private bool isGrounded = false;  // contact with the ground 
    private bool isCeilinged = false; // contact with the ceiling
    private bool isCrouched = false;
    private int  isWalled = 0;        // contact with the wall
    private bool isDead = false;
    private bool isHurting = false;

    // UI:
    private Text hpText;
    private Text gameoverText;


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
        gameoverText.gameObject.SetActive(false);
    }

    void Update()
    {
        CheckTheGround();
        CheckTheWall();
        CheckTheCeiling();

        Move();
        Crouch();
        Jump();
        FastFall();
        Attack();
        Animate();
    }



    private void CheckTheGround()
    {
        // Checking if player is on the ground:
        Collider2D collider = Physics2D.OverlapCircle(groundChecker.position, groundCheckerRadius, groundLayer);
        isGrounded = (collider != null);
    }

    // The Player Transform consists of two wall checkers so we know if the wall is on the right or on the left.
    private void CheckTheWall()
    {
        // Checking if player is on the wall:
        Collider2D collider = Physics2D.OverlapCircle(wallCheckerRight.position, wallCheckerRadius, groundLayer);
        if (collider != null) { isWalled = -1; }
        else
        {
            collider = Physics2D.OverlapCircle(wallCheckerLeft.position, wallCheckerRadius, groundLayer);
            isWalled = (collider != null) ? 1 : 0;
        }
    }

    private void CheckTheCeiling()
    {
        // Checking if there is a ceiling above the player:
        Collider2D collider = Physics2D.OverlapCircle(ceilingChecker.position, ceilingCheckerRadius, groundLayer);
        isCeilinged = (collider != null);
    }



    private void Move() // Run and Climb
    {
        // Run (horizontal movement):
        float xMove = Input.GetAxisRaw("Horizontal");
        rigbody.velocity = new Vector2(xMove * model.Speed, rigbody.velocity.y);

        // Flip:
        if (view.LookRight == true && xMove < 0)
        {
            playerGraphics.GetComponent<SpriteRenderer>().flipX = true;
            view.LookRight = false;
        }
        if (view.LookRight == false && xMove > 0)
        {
            playerGraphics.GetComponent<SpriteRenderer>().flipX = false;
            view.LookRight = true;
        }

        // Climb (vertical movement):
        if (isWalled != 0 && !isCeilinged)
        {
            float yMove = Input.GetAxisRaw("Vertical");
            if (yMove != 0)
                rigbody.velocity = new Vector2(rigbody.velocity.x, yMove * model.Speed);
            else if (xMove == 0)
                rigbody.velocity = new Vector2(-isWalled * 4.0f, 0.0f); // avoid sliding down 
        }
    }



    private void Jump()
    {
        if (isGrounded || isWalled != 0) canSomerSault = true;

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            attackViewNumber = -1; // reseting standard attacks (view) while in air

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

    private void FastFall() // after clicking DownArrow (or S), player goes down faster
    {
        if (!isGrounded && (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)))
            rigbody.velocity = new Vector2(0, -model.JumpForce / 1.5f);
    }



    private void Crouch()
    {
        if (isGrounded)
        {
            // if the collider's size reduces to 1/2 of original size, offset needs to go down 
            // for 1/4 of original size; "or isCeilinged" helps in situations when there is still 
            // a ceiling above player (but user stopped holding button)
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) || isCeilinged)
            {
                if (!isCrouched)
                {
                    capscol.size = new Vector2(capscol.size.x, capscol.size.y / 2);
                    capscol.offset = new Vector2(capscol.offset.x, capscol.offset.y - (capscol.size.y / 2));
                    isCrouched = true;
                }
                return;
            }

            // standing after crouching
            if (isCrouched)
            {
                capscol.size = new Vector2(capscol.size.x, capscol.size.y * 2);
                capscol.offset = new Vector2(capscol.offset.x, capscol.offset.y + (capscol.size.y / 4));
                isCrouched = false;
            }
        }
    }



    private void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isWalled == 0 && !isAttacking)
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
    }


    private void Animate()
    {
        if (isHurting) view.Hurt();
        else if (isDead) view.Die();
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
        // Decrease health:
        if (model.HP > 0)
        {
            model.HP -= dmg;
            UpdateHPText();
        }

        // Animate:
        if (model.HP > 0)
        {
            isHurting = true;
            Invoke(nameof(StopHurting), 0.2f); // "Hurt" animation will last for 0.2 seconds
        }
        else
        {
            isDead = true;
            Invoke(nameof(GameOver), animationLength * 2.1f); // Game freezes after Die animation
        }
    }

    private void StopHurting()
    {
        isHurting = false;
    }

    private void GameOver()
    {
        gameoverText.gameObject.SetActive(true);
        Time.timeScale = 0;    // freeze everything
    }

    private void UpdateHPText()
    {
        hpText.text = $"HEALTH POINTS: {model.HP} / {model.MaxHP}";
    }
}