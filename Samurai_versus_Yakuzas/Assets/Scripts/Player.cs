using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 4.0f;
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float fallMultiplier = 1.2f;
    [SerializeField] private float lowJumpMultiplier = 1.0f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float attackAnimSpeed = 4.0f;

    public int maxHealth = 10;
    public int currentHealth;
    public HealthBar healthBar;

    private Animator animator;
    private Rigidbody2D body2d;
    private Sensor groundSensor;
    private bool grounded = false;
    private bool combatIdle = false;
    private bool isDead = false;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private int jumpCount = 0;
    private const int maxJumps = 2;

    void Start()
    {
        animator = GetComponent<Animator>();
        body2d = GetComponent<Rigidbody2D>();
        groundSensor = transform.Find("GroundSensor").GetComponent<Sensor>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        HandleTimers();
        HandleJumping();
        HandleMovement();
        HandleAnimations();
    }

    private void HandleTimers()
    {
        if (grounded)
        {
            coyoteTimeCounter = coyoteTime;
            jumpCount = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    private void HandleJumping()
    {
        if ((jumpBufferCounter > 0f && coyoteTimeCounter > 0f) || (jumpBufferCounter > 0f && jumpCount < maxJumps))
        {
            body2d.velocity = new Vector2(body2d.velocity.x, jumpForce);
            animator.SetTrigger("Jump");
            grounded = false;
            animator.SetBool("Grounded", grounded);
            groundSensor.Disable(0.2f);
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
            jumpCount++;
        }

        if (body2d.velocity.y < 0f)
        {
            body2d.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (body2d.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            body2d.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    private void HandleMovement()
    {
        float inputX = Input.GetAxis("Horizontal");

        if (inputX > 0)
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else if (inputX < 0)
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        body2d.velocity = new Vector2(inputX * speed, body2d.velocity.y);
        animator.SetFloat("AirSpeed", body2d.velocity.y);

        if (!grounded && groundSensor.State())
        {
            grounded = true;
            animator.SetBool("Grounded", grounded);
        }
        else if (grounded && !groundSensor.State())
        {
            grounded = false;
            animator.SetBool("Grounded", grounded);
        }
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            animator.SetTrigger("Death");
            Invoke("Recover", 1);
        }
    }

    private void Recover()
    {
        currentHealth = maxHealth;
        animator.SetTrigger("Recover");
        healthBar.SetHealth(currentHealth);
    }

    private void HandleAnimations()
    {
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown("Hurt"))
        {
            animator.SetTrigger("Hurt");
            TakeDamage(1);
        }
        else if (Input.GetKeyDown(KeyCode.F) || Input.GetButtonDown("Parad"))
        {
            combatIdle = !combatIdle;
        }
        else if (Mathf.Abs(Input.GetAxis("Horizontal")) > Mathf.Epsilon)
        {
            animator.SetInteger("AnimState", 2);
        }
        else if (combatIdle)
        {
            animator.SetInteger("AnimState", 1);
        }
        else
        {
            animator.SetInteger("AnimState", 0);
        }
    }
}
