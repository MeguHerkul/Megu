using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public int health = 3;
    public GameObject[] hearts;
    public float playerSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Dash")]
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private bool canDash = true;
    private bool isDashing;

    public bool hasDashAbility = false;

    [Header("Wall Mechanics")]
    public float wallCheckDistance = 0.45f;
    public float wallSlideSpeed = 2f;

    public Vector2 wallJumpForce = new Vector2(10f, 15f); 
    public float wallJumpDuration = 0.2f; 
    private bool isWallJumping;

    private bool isTouchingWall;
    private bool isWallSliding;

    private Rigidbody2D rb;
    private bool isGrounded;

    private Animator animator;

    private SpriteRenderer spriteRenderer;

    public int extraJumpsValue = 0;
    private int extraJumps;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        extraJumps = extraJumpsValue;
        
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health) hearts[i].SetActive(true);
            else hearts[i].SetActive(false);
        }
    }


    void Update()
    {
        if (isDashing || isWallJumping) return;
        
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * playerSpeed, rb.linearVelocity.y);

        if (isGrounded)
        {
            extraJumps = extraJumpsValue;
        }

        if (moveInput > 0) 
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput < 0) 
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isWallSliding)
            {
                StartCoroutine(WallJump());
            }
            else if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            else if (extraJumps > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                extraJumps--;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && hasDashAbility)
        {
            StartCoroutine(Dash());
        }
       
        HandleWallSlide(moveInput);
        
        SetAnimation(moveInput);
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        Vector2 direction = new Vector2(transform.localScale.x, 0);
        isTouchingWall = Physics2D.Raycast(transform.position, direction, wallCheckDistance, groundLayer);
    }

    private IEnumerator WallJump()
    {
        isWallJumping = true;
        isWallSliding = false;

        transform.SetParent(null);

        float jumpDirection = -transform.localScale.x;
        rb.linearVelocity = new Vector2(wallJumpForce.x * jumpDirection, wallJumpForce.y);
        transform.localScale = new Vector3(jumpDirection, 1, 1);

        yield return new WaitForSeconds(wallJumpDuration);
        isWallJumping = false;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        transform.SetParent(null);
        
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashForce, 0f);
        yield return new WaitForSeconds(dashDuration);
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void SetAnimation(float moveInput)
    {
        if (isGrounded)
        {
            if (moveInput == 0) animator.Play("Player_Idle");
            else animator.Play("Player_Run");
        }
        else
        {
           
            if (isWallSliding)
            {
                animator.Play("Wall_Slide");
            }
            else if (rb.linearVelocityY > 0)
            {
                animator.Play("Player_Jump");
            }
            else
            {
                animator.Play("Player_Fall");
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    
    {
        if (collision.gameObject.tag == "Damage")
        {
            transform.SetParent(null);

            health -= 1; 

            for (int i = 0; i < hearts.Length; i++)
            {
                if (i < health) hearts[i].SetActive(true);
                else hearts[i].SetActive(false);
            }

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            StartCoroutine(BlinkRed());

            if (health <= 0)
            {
                StartCoroutine(DieSafe());
            }
        }
        if (collision.gameObject.CompareTag("Platform"))
        {
            transform.SetParent(collision.transform);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
           
            if (collision.gameObject.activeInHierarchy)
            {
                transform.SetParent(null);
            }
            else
            {                
                transform.parent = null;
            }
        }
    }
    private IEnumerator BlinkRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    private IEnumerator DieSafe()
    {
        transform.SetParent(null); 
        yield return null; 
        SceneManager.LoadScene("GameScene");
    }

    public void EnableDoubleJump()
   {
        extraJumpsValue = 1;
        Debug.Log("Double jump ability unlocked");
   }

    public void EnableDash()
    {
        hasDashAbility = true;
        Debug.Log("Dash ability unlocked");
    }

    private void HandleWallSlide(float moveInput)
    {
        
        bool pushingWall = (moveInput > 0 && transform.localScale.x > 0) || (moveInput < 0 && transform.localScale.x < 0);

        if (isTouchingWall && !isGrounded && pushingWall && rb.linearVelocityY < 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
        }
        else
        {
            isWallSliding = false;
        }
    }
}
