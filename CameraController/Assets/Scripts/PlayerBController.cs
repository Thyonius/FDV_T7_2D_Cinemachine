using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerBController : MonoBehaviour
{
    [SerializeField] float playerSpeed = 5.0f;
    [SerializeField] float jumpForce = 5.0f;
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] float attackRange = 0.2f;

    [SerializeField] LayerMask whatIsGround;
    [SerializeField] LayerMask whatIsEnemy;

    [SerializeField] Transform groundCheck;
    [SerializeField] Transform attackPoint;

    [SerializeField] NoiseSettings attackSignal;
    [SerializeField] float atkAmplitude = 0.1f;
    [SerializeField] float atkFrequency = 2.0f;
    [SerializeField] NoiseSettings damageSignal;
    [SerializeField] float dmgAmplitude = 0.5f;
    [SerializeField] float dmgFrequency = 5.0f;

    float movementInput;

    bool canMove = true;
    bool isGrounded;
    bool isFacingRight = true;

    Animator anim;
    Rigidbody2D rb;

    CinemachineImpulseSource myImpulseSource;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        myImpulseSource = GetComponent<CinemachineImpulseSource>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckSurroundings();
        CheckInput();
        CheckMovementDirection();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        UpdateAnimations();
    }

    private void CheckInput()
    {
        movementInput = Input.GetAxisRaw("HorizontalB");

        if (Input.GetButtonDown("JumpB") && isGrounded)
        {
            Jump();
        }
            
        if (Input.GetButtonDown("AttackB"))
        {
            BeginAttack();
        }
    }

    void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void ApplyMovement()
    {
        if (canMove)
        {
            rb.velocity = new Vector2(playerSpeed * movementInput, rb.velocity.y);
        }
    }

    private void CheckMovementDirection()
    {
        if (isFacingRight && movementInput < 0)
        {
            Flip();
        }
        else if (!isFacingRight && movementInput > 0)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void UpdateAnimations()
    {
        anim.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
        anim.SetBool("isGrounded", isGrounded);
    }

    void BeginAttack()
    {
        canMove = false;

        anim.SetTrigger("isAttack");
    }

    public void OnAttackCheckHit()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, whatIsEnemy);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<PlayerAController>().TakeDamage();

            myImpulseSource.m_ImpulseDefinition.m_RawSignal = attackSignal;
            myImpulseSource.m_ImpulseDefinition.m_AmplitudeGain = atkAmplitude;
            myImpulseSource.m_ImpulseDefinition.m_FrequencyGain = atkFrequency;
            myImpulseSource.GenerateImpulse();
        }
    }

    public void EndAttack()
    {
        canMove = true;
    }

    public void TakeDamage()
    {
        anim.SetTrigger("isHurt");

        myImpulseSource.m_ImpulseDefinition.m_RawSignal = damageSignal;
        myImpulseSource.m_ImpulseDefinition.m_AmplitudeGain = dmgAmplitude;
        myImpulseSource.m_ImpulseDefinition.m_FrequencyGain = dmgFrequency;
        myImpulseSource.GenerateImpulse();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        if (attackPoint == null)
        {
            return;
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
