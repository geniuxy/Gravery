using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCatchSwordState : PlayerState
{
    private Transform sword;

    public PlayerCatchSwordState(PlayerStateMachine _stateMachine, Player _player, string _animBoolName) : base(
        _stateMachine, _player, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // 有的时候会创建出两把剑
        // (已解决) 通过在ground状态下无法在isBusy情况下进入aimSword状态来解决
        sword = player.sword.transform;

        if (player.transform.position.x > sword.position.x && player.facingDir > 0)
            player.Flip();
        else if (player.transform.position.x < sword.position.x && player.facingDir < 0)
            player.Flip();

        AudioManager.instance.PlaySFX(29);
        player.fx.PlayDustFX();
        player.fx.ScreenShock(player.fx.catchSwordShockPower);

        rb.velocity = new Vector2(player.swordReturnImpact * -player.facingDir, rb.velocity.y);
    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }

    public override void Exit()
    {
        base.Exit();

        SkillManager.instance.sword.UseSkill();
        player.StartCoroutine("BusyFor", .1f);
    }
}