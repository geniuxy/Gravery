using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum SlimeType
{
    big,
    medium,
    small
}

public class Enemy_Slime : Enemy
{
    [Header("Split info")]
    [SerializeField] private SlimeType slimeType;
    [SerializeField] private int splitAmount;
    [SerializeField] private GameObject splitPrefab;
    [SerializeField] private Vector2 minCreatedVelocity;
    [SerializeField] private Vector2 maxCreatedVelocity;

    # region States

    public SlimeIdleState idleState { get; private set; }
    public SlimeMoveState moveState { get; private set; }

    public SlimeAttackState attackState { get; private set; }

    public SlimeBattleState battleState { get; private set; }

    public SlimeStunnedState stunnedState { get; private set; }

    public SlimeDeadState deadState { get; private set; }

    # endregion

    protected override void Awake()
    {
        base.Awake();

        idleState = new SlimeIdleState(stateMachine, this, this, "Idle");
        moveState = new SlimeMoveState(stateMachine, this, this, "Move");
        attackState = new SlimeAttackState(stateMachine, this, this, "Attack");
        battleState = new SlimeBattleState(stateMachine, this, this, "Move");
        stunnedState = new SlimeStunnedState(stateMachine, this, this, "Stunned");
        deadState = new SlimeDeadState(stateMachine, this, this, "Dead");
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

        if (slimeType == SlimeType.small)
            return;
        SplitSlime(splitAmount, splitPrefab);
    }

    private void SplitSlime(int _splitAmount, GameObject _splitPrefab)
    {
        for (int i = 0; i < _splitAmount; i++)
        {
            GameObject newSlime = Instantiate(_splitPrefab, transform.position, Quaternion.identity);
            StartCoroutine(InitSlime(newSlime));
        }
    }

    private IEnumerator InitSlime(GameObject newSlime)
    {
        yield return new WaitForSeconds(.1f);
        newSlime.GetComponent<Enemy_Slime>().SetupSlime(facingDir);
    }

    private void SetupSlime(int _facingDir)
    {
        if (facingDir != _facingDir)
            Flip();

        float xVelocity = Random.Range(minCreatedVelocity.x, maxCreatedVelocity.x);
        float yVelocity = Random.Range(minCreatedVelocity.y, maxCreatedVelocity.y);

        isKnocked = true;
        GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity * -facingDir, yVelocity);
        Invoke(nameof(CancelSlimeKnocked), 1.5f);
    }

    private void CancelSlimeKnocked() => isKnocked = false;
}