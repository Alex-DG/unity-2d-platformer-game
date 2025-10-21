using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem; // New Input System
using UnityEngine.SceneManagement;
using UnityEngine.UI; // add at top of file
public class Player : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float jumpForce = 10f;
    public float groundCheckRadius = 0.2f;
    public float deathShrinkDuration = 0.25f; // seconds
    public float maxFallDistance = 10f; // Maximum distance player can fall before dying
    public float minFallDistance = 5f; // Minimum fall distance that always triggers death (for low platforms)

    public int extaJumpValue = 1;
    public int health = 100;
    public int coins = 0;

    public Transform groundCheck;

    public LayerMask groundLayer;

    public Image healthImage;

    private Rigidbody2D rb;

    private float moveInput; // -1..1

    private int extaJumps;

    private Vector3 originalScale;

    private bool isDying;
    private bool isGrounded;

    private Animator animator;

    private SpriteRenderer spriteRenderer;

    // Falling detection variables
    private float highestYPosition;
    private bool hasReachedGround;
    private float fallStartY; // Y position when player started falling

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        extaJumps = extaJumpValue;
        originalScale = transform.localScale;
        isDying = false;

        // Initialize falling detection
        highestYPosition = transform.position.y;
        hasReachedGround = true;
    }


    #region Updates
    private void UpdateHealth()
    {
        healthImage.fillAmount = health / 100f;
    }

    private void UpdateAnimation()
    {
        if (isGrounded)
        {
            if (moveInput == 0)
            {
                // Idle
                animator.Play("Player_Idle");
            }
            else
            {
                // Run
                animator.Play("Player_Run");
            }
        }
        else
        {
            if (rb.linearVelocityY > 0)
            {
                // Jump
                animator.Play("Player_Jump");
            }
            else
            {
                // Fall
                animator.Play("Player_Fall");
            }
        }
    }

    void UpdateMoveInput(Keyboard k)
    {
        // A/Left = -1, D/Right = +1
        float left = (k.aKey.isPressed || k.leftArrowKey.isPressed) ? -1f : 0f;
        float right = (k.dKey.isPressed || k.rightArrowKey.isPressed) ? 1f : 0f;

        moveInput = Mathf.Clamp(left + right, -1f, 1f);
    }

    void UpdateVelocity()
    {
        var k = Keyboard.current;
        if (k == null) return;

        UpdateMoveInput(k);

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (isGrounded)
        {
            extaJumps = extaJumpValue;
        }

        if (k.spaceKey.wasPressedThisFrame)
        {

            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            else if (extaJumps > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                extaJumps--;

            }

        }
    }

    void Update()
    {
        UpdateVelocity();
        UpdateAnimation();
        UpdateHealth();
        CheckForFall();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
    #endregion

    #region States
    private void CheckForFall()
    {
        if (isDying) return;

        float currentY = transform.position.y;

        // Update highest position when player goes up
        if (currentY > highestYPosition)
        {
            highestYPosition = currentY;
            hasReachedGround = false; // Player is climbing, reset ground status
        }

        // When player becomes airborne, record the starting position
        if (!isGrounded && hasReachedGround)
        {
            fallStartY = currentY;
            hasReachedGround = false;
        }

        // Check if player has reached ground after falling
        if (isGrounded && !hasReachedGround)
        {
            hasReachedGround = true;
            // Reset the highest position to current position when landing
            highestYPosition = currentY;
        }

        // Check fall distance when player is airborne
        if (!isGrounded && !hasReachedGround)
        {
            float fallDistance = highestYPosition - currentY;
            float simpleFallDistance = fallStartY - currentY;

            // Use the larger of the two fall distances for more robust detection
            float actualFallDistance = Mathf.Max(fallDistance, simpleFallDistance);

            // Check against both minimum and maximum fall distances
            if (actualFallDistance > maxFallDistance ||
                (actualFallDistance > minFallDistance && highestYPosition - fallStartY < 2f)) // Low platform fall
            {
                Die();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Damage")
        {
            health -= 25;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            StartCoroutine(BlinkRed());

            if (health <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        if (isDying) return;
        isDying = true;
        StartCoroutine(ShrinkThenReload());
    }
    #endregion


    #region Interface

    private IEnumerator BlinkRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    private IEnumerator ShrinkThenReload()
    {
        // stop motion & inputs while dying (keeps your movement code intact)
        moveInput = 0f;
        rb.linearVelocity = Vector2.zero;   // keep your style
        rb.simulated = false;               // optional: freezes physics pushes during shrink

        Vector3 start = originalScale;
        Vector3 end = Vector3.zero;
        float t = 0f;

        // quick “squash” feel: ease-in
        while (t < deathShrinkDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / deathShrinkDuration);
            // ease: a^2 for a soft start
            float eased = a * a;
            transform.localScale = Vector3.LerpUnclamped(start, end, eased);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        // reload scene (fresh instance restores original scale)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion
}