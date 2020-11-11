// TO DO: 
// 1. FIX MOVEMENT (SLOPES)
// 2. ADD DASH (ARROW_DOWN ON SLOPES?)
// 3. ADD DYING AND HURTING INSTRUCTIONS AND ANIMATIONS

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerModel model; // speed, jump force, health points
    private PlayerView view;   // animations

    private Rigidbody2D rigbody;      // for movement
    private BoxCollider2D boxcol;     // for crouching
    private Transform playerGraphics; // for displaying

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
    private readonly float wallCorrectionParam = 0.2f;
    private bool isAttacking = false;
    private bool isSomerSaulting = false;
    private bool isGrounded = false;  // contact with the ground 
    private bool isCeilinged = false; // contact with the ceiling -> crouched
    private bool isCrouched = false;
    private int  isWalled = 0;        // contact with the wall
    private int  jumpedTimes = 0;
    private int  attackedTimes = -1;



    private void Awake()
    {
        playerGraphics = transform.Find("Graphics");
        view = playerGraphics.GetComponent<PlayerView>();

        model   = GetComponent<PlayerModel>();
        rigbody = GetComponent<Rigidbody2D>();
        boxcol  = GetComponent<BoxCollider2D>(); 
        groundChecker    = transform.Find("GroundChecker");
        ceilingChecker   = transform.Find("CeilingChecker");
        wallCheckerLeft  = transform.Find("WallCheckerLeft");
        wallCheckerRight = transform.Find("WallCheckerRight");
        hitPointLeft     = transform.Find("HitPointLeft").gameObject;
        hitPointRight    = transform.Find("HitPointRight").gameObject;
        hitPointRight.SetActive(false);
        hitPointLeft.SetActive(false);
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
        isGrounded = (collider != null) ? true : false;
    }

    // The Player Transform consists of two wall checkers so we know if the wall is on the right or on the left.
    // Thanks to this information we could implement bouncing behaviour, but... it didn't work. So I leave it 
    // for the future steps.
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
        isCeilinged = (collider != null) ? true : false;
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
        if (isWalled != 0)
        {
            float yMove = Input.GetAxisRaw("Vertical");
            if (yMove != 0)
                rigbody.velocity = new Vector2(rigbody.velocity.x, yMove * model.Speed);
            else
                rigbody.velocity = new Vector2(rigbody.velocity.x, wallCorrectionParam); // something is wrong
        }
    }



    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            attackedTimes = -1;

            if (isGrounded) jumpedTimes = 0;

            // Wall jump:
            if (isWalled != 0) // there is a wall on the left (-1) or right (1)
            {
                rigbody.velocity = new Vector2(rigbody.velocity.x, model.JumpForce);
                jumpedTimes = 1;
                return;
            }

            // Standard jump (from ground or double-jump):
            if (isGrounded || jumpedTimes < 2)
            {
                rigbody.velocity = new Vector2(rigbody.velocity.x, model.JumpForce);
                SomerSault();
                jumpedTimes++;
            }
        }
    }

    private void SomerSault()
    {
        if (jumpedTimes == 1)
        {
            isSomerSaulting = true;

            view.SomerSault();
            Invoke(nameof(SomerSaultCompleted), animationLength * 2);
        }
    }

    private void SomerSaultCompleted() { isSomerSaulting = false; }

    private void FastFall() // after clicking DownArrow (or W), player goes down faster
    {
        if (!isGrounded && (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.W)))
            rigbody.velocity = new Vector2(0, -model.JumpForce / 1.5f);
    }



    private void Crouch()
    {
        if (isGrounded)
        {
            // if the collider's size reduces to 1/2 of original size, offset needs to go down 
            // for 1/4 of original size; "or isCeilinged" helps in situations when there is still 
            // a ceiling above player (but user stopped holding button)
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || isCeilinged)
            {
                if (!isCrouched)
                {
                    boxcol.size = new Vector2(boxcol.size.x, boxcol.size.y / 2);
                    boxcol.offset = new Vector2(boxcol.offset.x, boxcol.offset.y - (boxcol.size.y / 2));
                    isCrouched = true;
                }
                return;
            }

            // standing after crouching
            if (isCrouched)
            {
                boxcol.size = new Vector2(boxcol.size.x, boxcol.size.y * 2);
                boxcol.offset = new Vector2(boxcol.offset.x, boxcol.offset.y + (boxcol.size.y / 4));
                isCrouched = false;
            }
        }
    }



    private void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isWalled == 0)
        {
            isAttacking = true;
            attackedTimes = (attackedTimes + 1) % 3;   // there are 3 types of attack

            Invoke(nameof(AttackStart), animationLength / 2.0f); // hit in the half of animation
            Invoke(nameof(AttackStop), animationLength);
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
        if (isWalled != 0)
        {
            if (rigbody.velocity.y > wallCorrectionParam) view.Climb();
            else if (rigbody.velocity.y == wallCorrectionParam) view.WallHold();
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
                    if (attackedTimes == 0) view.Attack1();
                    else if (attackedTimes == 1) view.Attack2();
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
        model.HP -= dmg;
        if (model.HP < 0)
        {
            // die
        }
        else
        {
            // hurt
        }
    }
}