// TO DO: 
// 1. ADD WALL JUMP BOUNCING BEHAVIOUR
// 2. ADD DASH
// 3. ADD MORE ATTACKS (WHILE STANDING AS WELL AS WHILE JUMPING OR CROUCHING)
// 4. FIX MOVEMENT (SLOPES)
// 5. ADD CROUCHING-MOVING ANIMATION

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
    private bool isAttacking = false;
    private bool isSomerSaulting = false;
    private bool isGrounded = false;  // contact with the ground 
    private bool isCeilinged = false; // contact with the ceiling -> crouched
    private bool isCrouched = false;
    private int  isWalled = 0;        // contact with the wall
    private int  jumpedTimes = 0;



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



    private void Move()
    {
        // Move (left-right):
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
    }



    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (isGrounded) jumpedTimes = 0;

            // Wall jump:
            if (isWalled != 0)
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



    private void Crouch()
    {
        // for faster falling or something like that can be another function - this one is only for crouching on the ground:
        if (isGrounded)
        {
            // if the collider's size reduces to 1/2 of original size, offset needs to go down for 1/4 of original size;
            // "or isCeilinged" helps in situations when there is still a ceiling above player (but user stopped holding button)
            if (Input.GetKey(KeyCode.DownArrow) || isCeilinged)
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isAttacking = true;

            if (view.LookRight) hitPointRight.SetActive(true);
            else hitPointLeft.SetActive(true);

            Invoke(nameof(AttackCompleted), animationLength);
        }
    }

    private void AttackCompleted()
    {
        isAttacking = false; 
        hitPointRight.SetActive(false);
        hitPointLeft.SetActive(false);
    }



    private void Animate()
    {
        if (isCrouched) view.Crouch();
        else if (isAttacking) view.Attack1();
        else if (isGrounded)
        {
            if (rigbody.velocity.x != 0) view.Run();
            else view.Idle();
        }
        else
        {
            if (isSomerSaulting) view.SomerSault();
            else if (rigbody.velocity.y > 0) view.Jump();
            else view.Fall();
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