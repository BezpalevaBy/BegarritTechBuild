using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int IsMovingAttack = Animator.StringToHash("IsMovingAttack");
    private static readonly int MovingWhileAttack = Animator.StringToHash("MovingWhileAttack");
    private static readonly int IsMovementBlocked = Animator.StringToHash("IsMovementBlocked");
    [SerializeField] private float maxSpeed = 2;
    [SerializeField] private float accelPower = 0.1f;
    [SerializeField] private float jumpPower = 1f;
    private Rigidbody2D rb;
    private float lastJumpTime = 0;
    private float jumpCooldown = 2;
    private Collider2D groundCollider;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundCollider = GameObject.FindWithTag("ground").GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!IsGrounded()) return;
        if (StopGameManager.IsPaused) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        
        if ((Math.Abs(moveX) < 0.1f))
        {
            animator.SetBool(IsMovingAttack, false);
            animator.ResetTrigger(MovingWhileAttack);
        }
        else
        {
            animator.SetTrigger(MovingWhileAttack);
            animator.SetBool(IsMovingAttack, true);
        }

        if (animator.GetBool(IsMovementBlocked)) return;

        if (Input.GetAxisRaw("Vertical") > 0)
        {
            Jump();
            return;
        }

        HandleMovement(moveX);
    }

    bool IsGrounded()
    {
        return rb.IsTouching(groundCollider);
    }

    void HandleMovement(float moveX)
    {
        animator.SetFloat(Speed, Mathf.Abs(rb.velocity.x));
        
        if (Mathf.Abs(moveX) < 0.1f)
        {
            rb.velocity = new Vector2(
                Mathf.Lerp(rb.velocity.x, 0, 3 * Time.deltaTime),
                rb.velocity.y
            );
            return;
        }

        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.velocity += new Vector2(moveX * accelPower, 0);
        }
        if (moveX > 0) transform.localScale = new Vector3(Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (moveX < 0) transform.localScale = new Vector3(-1*Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    void Jump()
    {
        if (Time.realtimeSinceStartup - lastJumpTime < jumpCooldown) return;
        lastJumpTime = Time.realtimeSinceStartup;
        rb.velocity = new Vector2(rb.velocity.x*5, jumpPower);
    }
    
    public float dashForce = 1000f;
    
    public void StartDash() {
        // Импульс вперед в зависимости от того, куда смотрит персонаж
        Vector2 direction = new Vector2(transform.localScale.x, 0); 
        rb.AddForce(direction * dashForce, ForceMode2D.Impulse);
    }

    public void EndDash() {
        rb.velocity = Vector2.zero; // Резкая остановка после выпада
    }
}