using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    private Blackhole_Skill blackholeSkill;
    private Sword_Skill sword;
    private Parry_Skill parry;

    public PlayerGroundedState(PlayerStateMachine _stateMachine, Player _player, string _animBoolName) : base(
        _stateMachine, _player, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        blackholeSkill = player.skill.blackhole;
        sword = player.skill.sword;
        parry = player.skill.parry;
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.R) && blackholeSkill.blackholeUnlock && blackholeSkill.CanUseSkill())
            stateMachine.ChangeState(player.blackholeState);

        if (Input.GetKeyDown(KeyCode.Mouse1) && HasNoSword() && sword.CanUseSkill() && sword.swordUnlock &&
            !player.isBusy)
            stateMachine.ChangeState(player.aimSwordState);

        if (Input.GetKeyDown(KeyCode.Q) && parry.CanUseSkill() && parry.parryUnlock)
            stateMachine.ChangeState(player.counterAttackState);

        if (!player.IsGroundDetected())
            stateMachine.ChangeState(player.airState);

        if (Input.GetKeyDown(KeyCode.Mouse0))
            stateMachine.ChangeState(player.primaryAttackState);

        if (Input.GetKeyDown(KeyCode.Space) && player.IsGroundDetected() && !player.isBusy)
            stateMachine.ChangeState(player.jumpState);
    }

    public override void Exit()
    {
        base.Exit();
    }

    private bool HasNoSword()
    {
        if (!player.sword)
            return true;
        player.sword.GetComponent<Sword_Skill_Controller>().ReturnSword();
        return false;
    }
}