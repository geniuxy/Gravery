using UnityEngine;

public class DeathBringerIdleState : DeathBringerGroundedState
{
    public DeathBringerIdleState(EnemyStateMachine _stateMachine, Enemy _enemyBase, Boss_DeathBringer _enemy,
        string _animBoolName) : base(_stateMachine, _enemyBase, _enemy, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.idleTime;
    }

    public override void Update()
    {
        base.Update();

        enemy.isBossFightBegin =
            Vector2.Distance(player.position, enemy.transform.position) < enemy.battleDistance * 1.5f;

        if (stateTimer < 0 && enemy.isBossFightBegin)
            stateMachine.ChangeState(enemy.moveState);
    }

    public override void Exit()
    {
        base.Exit();

        AudioManager.instance.PlaySFX(15, enemy.transform);
    }
}