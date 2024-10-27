using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Shady : Enemy
{
    [Header("Shady Variables")]
    public float battleSpeed;
    [SerializeField] private GameObject explosivePrefab;
    [SerializeField] private float growSpeed;
    [SerializeField] private float maxSize;

    # region States

    public ShadyIdleState idleState { get; private set; }
    public ShadyMoveState moveState { get; private set; }
    public ShadyBattleState battleState { get; private set; }
    public ShadyStunnedState stunnedState { get; private set; }
    public ShadyDeadState deadState { get; private set; }

    # endregion

    protected override void Awake()
    {
        base.Awake();

        idleState = new ShadyIdleState(stateMachine, this, this,"Idle");
        moveState = new ShadyMoveState(stateMachine, this, this, "Move");
        battleState = new ShadyBattleState(stateMachine, this, this, "MoveFast");
        stunnedState = new ShadyStunnedState(stateMachine, this, this, "Stunned");
        deadState = new ShadyDeadState(stateMachine, this, this, "Dead");
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

    public override void SpecialAttackTrigger()
    {
        GameObject newExplosive = Instantiate(explosivePrefab, transform.position, Quaternion.identity);
        newExplosive.GetComponent<Explosive_Controller>().SetupExplosive(stats, growSpeed, maxSize, attackCheckRadius);
        // 为了可以被穿过爆炸物，所以要关闭碰撞体
        cd.enabled = false;
        rb.gravityScale = 0;
    }

    public void SelfDestroy() => Destroy(gameObject);
}