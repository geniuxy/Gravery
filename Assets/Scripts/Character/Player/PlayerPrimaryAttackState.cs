using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    public int comboCounter { get; private set; }
    private Coroutine AttackBusyCoroutine;

    private float lastTimeAttack;
    private float comboWindow = 2f;

    public PlayerPrimaryAttackState(PlayerStateMachine _stateMachine, Player _player, string _animBoolName) : base(
        _stateMachine, _player, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if (comboCounter > 2 || lastTimeAttack < Time.time - comboWindow)
        {
            comboCounter = 0;
        }

        player.anim.SetInteger("ComboCounter", comboCounter);

        AudioManager.instance.PlaySFX(comboCounter);
        // 为了在攻击工作之间切换时，可以改变攻击方向
        float attackDir = xInput != 0 ? xInput : player.facingDir;
        // 为了攻击有一段小跳
        player.SetVelocity(player.attackMovement[comboCounter].x * attackDir, player.attackMovement[comboCounter].y);

        stateTimer = .1f;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
            player.SetZeroVelocity();

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }

    public override void Exit()
    {
        base.Exit();
        // 当协程还存在时，重置isBusy持续的时间
        if (AttackBusyCoroutine!=null)
            player.StopCoroutine(AttackBusyCoroutine);
        AttackBusyCoroutine= player.StartCoroutine("BusyFor", .1f);

        comboCounter++;
        lastTimeAttack = Time.time;
    }
}