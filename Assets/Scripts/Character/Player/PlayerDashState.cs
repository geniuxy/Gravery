using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(PlayerStateMachine _stateMachine, Player _player, string _animBoolName) : base(_stateMachine,
        _player, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        AudioManager.instance.PlaySFX(19);

        player.skill.dash.cloneOnDash();
        player.stats.MakeInvincible(true);

        stateTimer = player.dashDuration;
    }

    public override void Update()
    {
        base.Update();

        if (!player.IsGroundDetected()&&player.IsWallDetected())
            stateMachine.ChangeState(player.wallSlideState);

        player.SetVelocity(player.dashSpeed * player.dashDir, 0);

        if (stateTimer < 0)
            stateMachine.ChangeState(player.idleState);

        player.fx.CreateDashImageFX();
    }

    public override void Exit()
    {
        base.Exit();

        player.skill.dash.cloneOnArrival();
        player.stats.MakeInvincible(false);

        player.SetVelocity(0,rb.velocity.y);
    }
}