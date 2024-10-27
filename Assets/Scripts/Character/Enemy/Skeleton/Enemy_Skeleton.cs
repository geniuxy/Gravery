using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Skeleton : Enemy
{
    # region States

    public SkeletonIdleState idleState { get; private set; }
    public SkeletonMoveState moveState { get; private set; }
    public SkeletonBattleState battleState { get; private set; }
    public SkeletonAttackState attackState { get; private set; }
    public SkeletonStunnedState stunnedState { get; private set; }
    public SkeletonDeadState deadState { get; private set; }

    # endregion

    protected override void Awake()
    {
        base.Awake();

        idleState = new SkeletonIdleState(stateMachine, this, this, "Idle");
        moveState = new SkeletonMoveState(stateMachine, this, this, "Move");
        battleState = new SkeletonBattleState(stateMachine, this, this, "Move");
        attackState = new SkeletonAttackState(stateMachine, this, this, "Attack");
        stunnedState = new SkeletonStunnedState(stateMachine, this, this, "Stunned");
        deadState = new SkeletonDeadState(stateMachine, this, this, "Idle");
    }

    protected override void Start()
    {
        base.Start();

        // // 解决初始Animator sprite朝向左的问题
        // SetupDefaultFacingDir(-1);
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
}