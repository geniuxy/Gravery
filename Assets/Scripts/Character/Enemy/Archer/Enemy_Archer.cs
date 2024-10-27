using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Archer : Enemy
{
    [Header("Archer Variables")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float arrowSpeed;
    public Vector2 jumpForce;
    public float jumpCoolDown;
    public float saveDistance;
    [HideInInspector] public float lastTimeJump;

    [Header("Ground Behind Check")]
    [SerializeField] private Transform groundBehindCheck;
    [SerializeField] private Vector2 groundBehindCheckArea;

    # region States

    public ArcherIdleState idleState { get; private set; }
    public ArcherMoveState moveState { get; private set; }
    public ArcherBattleState battleState { get; private set; }
    public ArcherRangedAttackState rangedAttackState { get; private set; }
    public ArcherMeleeAttackState meleeAttackState { get; private set; }
    public ArcherDeadState deadState { get; private set; }
    public ArcherStunnedState stunnedState { get; private set; }
    public ArcherJumpState jumpState { get; private set; }

    # endregion

    protected override void Awake()
    {
        base.Awake();

        idleState = new ArcherIdleState(stateMachine, this, this, "Idle");
        moveState = new ArcherMoveState(stateMachine, this, this, "Move");
        battleState = new ArcherBattleState(stateMachine, this, this, "Idle");
        rangedAttackState = new ArcherRangedAttackState(stateMachine, this, this, "RangedAttack");
        meleeAttackState = new ArcherMeleeAttackState(stateMachine, this, this, "MeleeAttack");
        deadState = new ArcherDeadState(stateMachine, this, this, "Dead");
        stunnedState = new ArcherStunnedState(stateMachine, this, this, "Stunned");
        jumpState = new ArcherJumpState(stateMachine, this, this, "Jump");
    }

    protected override void Start()
    {
        base.Start();

        // 解决初始Animator sprite朝向左的问题
        SetupDefaultFacingDir(-1);
        // 怪的初始朝向希望与默认朝向不一致时，翻转
        if (defaultFacingDir != initFacingDir)
            Flip();

        stateMachine.Initialize(idleState);
    }

    public override bool CanBeStunned()
    {
        if (base.CanBeStunned())
        {
            stateMachine.ChangeState(stunnedState);
            return true;
        }

        return false;
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }

    public override void SpecialAttackTrigger()
    {
        base.SpecialAttackTrigger();

        GameObject arrow = Instantiate(arrowPrefab, attackCheck.position, transform.rotation);
        arrow.transform.Rotate(0, 180, 0);
        arrow.GetComponent<Arrow_Controller>().SetupArrow(arrowSpeed * facingDir, stats);
    }

    public bool GroundBehind() =>
        Physics2D.BoxCast(groundBehindCheck.position, groundBehindCheckArea, 0, Vector2.zero, 0, whatIsGround);

    // 找背后的墙，方向为Vector2.right * -facingDir
    public bool WallBehind() =>
        Physics2D.Raycast(wallCheck.position, Vector2.right * -facingDir, wallCheckDistance + 2, whatIsGround);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireCube(groundBehindCheck.position, groundBehindCheckArea);
        Gizmos.DrawLine(wallCheck.position,
            new Vector3(wallCheck.position.x + (wallCheckDistance + 2) * -facingDir, wallCheck.position.y));
    }
}