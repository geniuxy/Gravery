using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlackholeState : PlayerState
{
    private float flyTime = .4f;
    private bool skillUsed;
    private float defaultGravityScale;

    public PlayerBlackholeState(PlayerStateMachine _stateMachine, Player _player, string _animBoolName) : base(
        _stateMachine, _player, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = flyTime;
        skillUsed = false; // 这个不能省，每一次进入state都需要重置一下
        defaultGravityScale = rb.gravityScale;
        rb.gravityScale = 0;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
            rb.velocity = new Vector2(0, 15f);
        else
        {
            rb.velocity = new Vector2(0, -.1f);

            if (!skillUsed)
            {
                player.skill.blackhole.UseSkill();
                skillUsed = true;
            }
        }

        if (player.skill.blackhole.SkillCompleted())
            stateMachine.ChangeState(player.airState);
    }

    public override void Exit()
    {
        base.Exit();

        AudioManager.instance.StopSFX(7);
        rb.gravityScale = defaultGravityScale;
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }
}