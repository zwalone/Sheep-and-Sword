// TODO: FIX ATTACK, ADD WALL JUMP, ADD DASH

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

        Move();
        Jump();
        Attack();
    }

    private void CheckTheGround()
    {
        // Checking if players is on the ground:
        Collider2D collider = Physics2D.OverlapCircle(groundChecker.position, groundCheckerRadius, groundLayer);
        isGrounded = (collider != null) ? true : false;
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
        // Jump (from ground or double-jump):
        if (Input.GetKeyDown(KeyCode.UpArrow) && (isGrounded || jumpedTimes < 1))
        {
            rigbody.velocity = new Vector2(rigbody.velocity.x, jumpForce);
            jumpedTimes++;
        }
        if (isGrounded) jumpedTimes = 0;
    }

    // VERY BUGGY, IDK WHY
    private void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (lookRight) hitPointRight.SetActive(true);
            else hitPointLeft.SetActive(true);
        }
    }
}