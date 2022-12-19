using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private TrailRenderer tr;

    public int playerHealth, maxHealth;

    //movement
    public float speed;
    private float moveInput;
    public float jumpForce;

    //feet collider
    public Transform feetPos;
    public float checkRadius;
    public LayerMask whatIsGround;

    //for jump physics
    private float jumpTimeCounter;
    public float jumpTime;
    private bool isJumping;

    //animation
    private Animator animator;

    //dash
    private float dashingVelocity = 30f;
    private float dashingTime = 0.3f;
    private Vector2 dashingDirection;
    private bool isDashing;
    private bool canDash = true;

    //events
    public event Action onPlayerDamage;

    public event Action onPlayerAttack;



    // Start is called before the first frame update
    void Start()
    {
        reborn();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        tr = GetComponent<TrailRenderer>();
    }


    void Update()
    {
        ApplyAnimations();

        moveInput = Input.GetAxisRaw("Horizontal");
        var jumpInput = Input.GetKeyDown(KeyCode.Z);
        var jumpInputReleased = Input.GetKeyUp(KeyCode.Z);
        var dashInput = Input.GetKeyDown(KeyCode.X);

        if(rb.position.y < -25f || playerHealth <=0)
        {
            rb.position = respawn();
        }

        //dash physics
        if (dashInput && canDash)
        {
            isDashing = true;
            this.GetComponent<Collider2D>().isTrigger = true;
            canDash = false;
            tr.emitting = true;
            
            if (moveInput >= 0 && transform.rotation.y == 0)
            {
                dashingDirection= new Vector2(1, 0);
            }
            else
            {
                dashingDirection = new Vector2(-1, 0);
            }
            StartCoroutine(StopDashing());

        }
        animator.SetBool("IsDashing", isDashing);

        if (isDashing)
        {
            rb.velocity = dashingDirection * dashingVelocity;
            return;
        }

        //standing still
        if (IsGrounded())
        {
            canDash = true;

        }

        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        if (moveInput > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            dashingDirection= new Vector2(1, 0);
        }
        else if (moveInput < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            dashingDirection= new Vector2(-1, 0);
        }

        if(jumpInput && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if(jumpInputReleased && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
    }
    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        isDashing = false;
        this.GetComponent<Collider2D>().isTrigger = false;

    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
    }

    private void reborn()
    {
        playerHealth = maxHealth;
        onPlayerDamage?.Invoke();
    }
    Vector2 respawn()
    {
        reborn();
        return new Vector2(-8.5f, -2);
    }

    private void damage()
    {
        playerHealth--;
        onPlayerDamage?.Invoke();
    }

    private void ApplyAnimations()
    {
        animator.SetBool("IsWalking", moveInput != 0);
        animator.SetBool("IsGrounded", IsGrounded());
        animator.SetFloat("VerticalVelocity", rb.velocity.y);
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.layer != LayerMask.NameToLayer("Enemy"))
        {
            this.GetComponent<Collider2D>().isTrigger = false;

        }

        else if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (isDashing)
            {
                onPlayerAttack?.Invoke();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (!isDashing)
            {
                damage();
            }
        }
    }
}
