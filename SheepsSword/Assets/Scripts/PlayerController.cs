// TODO: ADD WALL JUMP, ADD DASH

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Movement parameters:
    public float speed;
    public float jumpForce;
    Rigidbody2D rigbody;

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

    // Animation correction parameter:
    bool lookRight = true;

    // Attack parameters:
    public GameObject hitPointRight;
    public GameObject hitPointLeft;

    void Start()
    {
        rigbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        hitPointRight.SetActive(false);
        hitPointLeft.SetActive(false);
        CheckTheGround();
        CheckTheWall();

        Move();
        Jump();
        Attack();
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

    private void Move()
    {
        // Move (left-right):
        float x = Input.GetAxisRaw("Horizontal");
        rigbody.velocity = new Vector2(x * speed, rigbody.velocity.y);

        // Flip:
        if (lookRight == true && x < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            lookRight = false;
        }
        if (lookRight == false && x > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            lookRight = true;
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

    private void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (lookRight) hitPointRight.SetActive(true);
            else hitPointLeft.SetActive(true);
        }
    }
}