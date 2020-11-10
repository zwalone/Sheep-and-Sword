// TO DO: ADD WALL JUMP BOUNCING BEHAVIOUR, ADD DASH
// TO REPAIR: ATTACK (AND IT'S ANIMATION)

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Basic movement parameters:
    public float speed;
    public float jumpForce;
    Rigidbody2D rigbody;
    Transform playerGraphics;
    bool lookRight = true;

    // Jumping correction parameters:
    int jumpedTimes = 0;
    bool isGrounded = false;
    public Transform groundChecker;
    public float groundCheckerRadius;
    public LayerMask groundLayer;

    // Wall jumping correction parameters:
    public Transform wallCheckerLeft;
    public Transform wallCheckerRight;
    public float wallCheckerRadius;
    int isWalled = 0;

    // Crouching parameters:
    public Transform ceilingChecker;
    bool isCeilinged = false;
    bool isCrouched = false;
    BoxCollider2D boxcol;
    public float ceilingCheckerRadius;

    // Attack parameters:
    public GameObject hitPointRight;
    public GameObject hitPointLeft;

    void Start()
    {
        rigbody = GetComponent<Rigidbody2D>();
        boxcol = GetComponent<BoxCollider2D>();
        playerGraphics = transform.Find("Graphics");
        if (playerGraphics == null) Debug.Log("Graphics component is missing!");
    }

    void Update()
    {
        hitPointRight.SetActive(false);
        hitPointLeft.SetActive(false);
        playerGraphics.GetComponent<Animator>().SetBool("attacked", false);
        CheckTheGround();
        CheckTheWall();
        CheckTheCeiling();

        Move();
        Crouch();
        Jump();
        Attack();
        Debug.Log("ciastko");
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
        float x = Input.GetAxisRaw("Horizontal");
        rigbody.velocity = new Vector2(x * speed, rigbody.velocity.y);
        playerGraphics.GetComponent<Animator>().SetFloat("speedX", System.Math.Abs(rigbody.velocity.x));

        // Flip:
        if (lookRight == true && x < 0)
        {
            playerGraphics.GetComponent<SpriteRenderer>().flipX = true;
            lookRight = false;
        }
        if (lookRight == false && x > 0)
        {
            playerGraphics.GetComponent<SpriteRenderer>().flipX = false;
            lookRight = true;
        }
    }

    private void Jump()
    {
        playerGraphics.GetComponent<Animator>().SetBool("isGrounded", isGrounded);
        playerGraphics.GetComponent<Animator>().SetFloat("speedY", rigbody.velocity.y);
        playerGraphics.GetComponent<Animator>().SetInteger("jumpedTimes", jumpedTimes);

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (isGrounded) jumpedTimes = 0;

            // Wall jump:
            if (isWalled != 0)
            {
                rigbody.velocity = new Vector2(rigbody.velocity.x, jumpForce);
                jumpedTimes = 1;
                return;
            }

            // Standard jump (from ground or double-jump):
            if (isGrounded || jumpedTimes < 2)
            {
                rigbody.velocity = new Vector2(rigbody.velocity.x, jumpForce);
                jumpedTimes++;
            }
        }
    }

    private void Crouch()
    {
        playerGraphics.GetComponent<Animator>().SetBool("isCrouched", isCrouched);
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

    // Sometimes it doesn't work...
    private void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (lookRight) hitPointRight.SetActive(true);
            else hitPointLeft.SetActive(true);

            playerGraphics.GetComponent<Animator>().SetBool("attacked", true);
        }
    }
}