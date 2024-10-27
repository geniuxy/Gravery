using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCounterAttackState : PlayerState
{
    private bool canCreateCloneAttack;

    public PlayerCounterAttackState(PlayerStateMachine _stateMachine, Player _player, string _animBoolName) : base(
        _stateMachine, _player, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        canCreateCloneAttack = player.skill.parry.parryWithMirageUnlock;
        stateTimer = player.counterAttackDuration;
        player.anim.SetBool("SuccessfulCounterAttack", false);
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();

        Collider2D[] collisions = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in collisions)
        {
            if (hit.GetComponent<Arrow_Controller>() != null)
            {
                SuccessToParry();
                hit.GetComponent<Arrow_Controller>().FlipArrow();
            }

            if (hit.GetComponent<Enemy>() != null)
            {
                if (hit.GetComponent<Enemy>().CanBeStunned())
                {
                    SuccessToParry();

                    if (canCreateCloneAttack)
                    {
                        canCreateCloneAttack = false; // 为了在反击多个敌人时，只创建一个clone
                        player.skill.parry.MakeMirageOnParry(hit.transform);
                    }
                }
            }
        }

        if (stateTimer < 0 || triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }

    private void SuccessToParry()
    {
        player.skill.parry.UseSkill();
        stateTimer = 2f; // 为了不让弹反的动作一下子就结束了
        player.anim.SetBool("SuccessfulCounterAttack", true);
        AudioManager.instance.PlaySFX(26);
    }

    public override void Exit()
    {
        base.Exit();
    }
}