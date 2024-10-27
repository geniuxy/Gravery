using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(EntityFX))]
[RequireComponent(typeof(ItemDrop))]
public class Enemy : Entity
{
    [SerializeField] protected LayerMask whatIsPlayer;

    [Header("Stunned info")]
    public float stunnedDuration = 2;
    public Vector2 stunnedDir = new Vector2(7, 12);
    protected bool canBeStunned;
    [SerializeField] protected GameObject counterImage;

    [Header("Move info")]
    public float moveSpeed = 1.5f;
    public float idleTime = 2f;
    private float defaultMoveSpeed;

    [Header("Attack info")]
    public float attackDistance = 1.2f;
    public float attackCoolDown = .4f;
    public float minAttackCoolDown = .25f;
    public float maxAttackCoolDown = .5f;
    public float battleDistance = 6;
    public float battleTime = 6;
    [HideInInspector] public float lastTimeAttacked;

    public EnemyStateMachine stateMachine { get; private set; }
    public EntityFX fx { get; private set; }
    public string lastAnimBoolName { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new EnemyStateMachine();

        defaultMoveSpeed = moveSpeed;
    }

    protected override void Start()
    {
        base.Start();

        fx = GetComponent<EntityFX>();
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
    }

    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        moveSpeed *= 1 - _slowPercentage;
        anim.speed *= 1 - _slowPercentage;

        Invoke(nameof(ReturnDefaultSpeed), _slowDuration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        moveSpeed = defaultMoveSpeed;
    }

    public virtual void AssignLastAnimBoolName(string _animBoolName) => lastAnimBoolName = _animBoolName;

    public virtual RaycastHit2D IsPlayerDetected()
    {
        RaycastHit2D playerDetected =
            Physics2D.Raycast(rb.position, Vector2.right * facingDir, battleDistance, whatIsPlayer);
        RaycastHit2D wallDetected =
            Physics2D.Raycast(rb.position, Vector2.right * facingDir, battleDistance - 1, whatIsGround);

        // if (wallDetected)
        //     return default(RaycastHit2D); // 返回一个默认值，即false
        return wallDetected.distance < playerDetected.distance && wallDetected ? default(RaycastHit2D) : playerDetected;
    }

    public virtual RaycastHit2D IsPlayerBehindDetected() =>
        Physics2D.Raycast(rb.position, Vector2.right * -facingDir, battleDistance * .5f, whatIsPlayer);

    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    public virtual void SpecialAttackTrigger()
    {
    }

    public void FreezeTimeFor(float _duration) => StartCoroutine(FreezeTimeCoroutine(_duration));

    protected virtual IEnumerator FreezeTimeCoroutine(float _seconds)
    {
        FreezeTime(true);
        yield return new WaitForSeconds(_seconds);
        FreezeTime(false);
    }

    public virtual void FreezeTime(bool _timeFrozen)
    {
        if (_timeFrozen)
        {
            // 移动速度为0
            moveSpeed = 0;
            // 动画速度为0
            anim.speed = 0;
        }
        else
        {
            moveSpeed = defaultMoveSpeed;
            anim.speed = 1;
        }
    }

    # region Counter Attack

    public virtual void OpenCounterAttackWindow()
    {
        canBeStunned = true;
        counterImage.SetActive(true);
    }

    public virtual void CloseCounterAttackWindow()
    {
        canBeStunned = false;
        counterImage.SetActive(false);
    }

    public virtual bool CanBeStunned()
    {
        if (canBeStunned)
        {
            CloseCounterAttackWindow();
            return true;
        }

        return false;
    }

    # endregion

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y - .1f),
            new Vector3(transform.position.x + attackDistance * facingDir, transform.position.y - .1f));
        // -.1f 是为了不重叠
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(transform.position.x - battleDistance * .5f * facingDir, transform.position.y),
            new Vector3(transform.position.x + battleDistance * facingDir, transform.position.y));
    }
}