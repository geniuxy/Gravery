using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Knockback info")]
    [SerializeField] protected Vector2 knockbackDir = new Vector2(5, 7);
    [SerializeField] protected Range knockbackOffsetRange = new Range(.5f, 2);
    [SerializeField] protected float knockbackDuration = .07f;
    protected bool isKnocked;

    [Header("Collision info")]
    public Transform attackCheck;
    public float attackCheckRadius = .8f;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance = 1f;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance = .15f;

    public int facingDir = 1;
    [SerializeField] protected int initFacingDir = 1;
    public int defaultFacingDir { get; private set; } = 1;
    protected bool facingRight = true;

    # region Components

    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }

    public SpriteRenderer sr { get; private set; }

    public CharacterStats stats { get; private set; }

    public CapsuleCollider2D cd { get; private set; }

    # endregion

    public System.Action onFlipped;

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<CharacterStats>();
        cd = GetComponent<CapsuleCollider2D>();
    }

    protected virtual void Update()
    {
    }

    # region Collision

    public virtual void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
    }

    protected virtual void ReturnDefaultSpeed() => anim.speed = 1;

    public virtual void DamageImpact(int _attackDir) => StartCoroutine(HitKnockback(_attackDir));

    protected virtual IEnumerator HitKnockback(int attackDir)
    {
        isKnocked = true;

        float xOffset = Random.Range(knockbackOffsetRange.min, knockbackOffsetRange.max);
        rb.velocity = new Vector2((knockbackDir.x + xOffset) * attackDir, knockbackDir.y);
        yield return new WaitForSeconds(knockbackDuration);

        isKnocked = false;
    }

    public void SetupZeroKnockback() => knockbackDir = Vector2.zero;

    public void SetupKnockbackDir(Vector2 _knockbackDir) => knockbackDir = _knockbackDir;

    public virtual bool IsGroundDetected() =>
        Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

    public virtual bool IsWallDetected() =>
        Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position,
            new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position,
            new Vector3(wallCheck.position.x + wallCheckDistance * facingDir, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }

    # endregion

    # region Flip

    public virtual void FlipController(float _x)
    {
        if ((_x > 0 && !facingRight) || (_x < 0 && facingRight))
            Flip();
    }

    public virtual void Flip()
    {
        facingDir *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);

        onFlipped?.Invoke();
    }

    protected virtual void SetupDefaultFacingDir(int _facingDir)
    {
        facingDir = _facingDir;
        defaultFacingDir = facingDir;

        if (facingDir == -1)
            facingRight = false;
    }

    # endregion

    # region Velocity

    public virtual void SetZeroVelocity()
    {
        if (isKnocked)
            return;

        rb.velocity = new Vector2(0, 0);
    }

    public virtual void SetVelocity(float _xVelocity, float _yVelocity)
    {
        if (isKnocked)
            return;

        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }

    # endregion

    public virtual void Die()
    {
    }
}