using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimSwordState : PlayerState
{
    public PlayerAimSwordState(PlayerStateMachine _stateMachine, Player _player, string _animBoolName) : base(
        _stateMachine, _player, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        SkillManager.instance.sword.DotsActive(true);
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();

        if (Input.GetKeyUp(KeyCode.Mouse1))
            stateMachine.ChangeState(player.idleState);

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (player.transform.position.x > mousePos.x && player.facingDir > 0)
            player.Flip();
        else if (player.transform.position.x < mousePos.x && player.facingDir < 0)
            player.Flip();
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", .5f);
    }
}