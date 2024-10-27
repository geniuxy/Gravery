using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeGroundedState : EnemyState
{
    protected Enemy_Slime enemy;

    protected Transform player;

    public SlimeGroundedState(EnemyStateMachine _stateMachine, Enemy _enemyBase, Enemy_Slime _enemy, string _animBoolName)
        : base(_stateMachine, _enemyBase, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerDetected() ||
            Vector2.Distance(player.position,
                enemy.transform.position) < enemy.battleDistance * .5f)
            stateMachine.ChangeState(enemy.battleState);
    }

    public override void Exit()
    {
        base.Exit();
    }
}