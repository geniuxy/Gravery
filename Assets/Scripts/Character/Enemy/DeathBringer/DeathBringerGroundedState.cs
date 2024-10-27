using UnityEngine;

public class DeathBringerGroundedState : EnemyState
{
    protected Boss_DeathBringer enemy;

    protected Transform player;

    public DeathBringerGroundedState(EnemyStateMachine _stateMachine, Enemy _enemyBase, Boss_DeathBringer _enemy,
        string _animBoolName) : base(_stateMachine, _enemyBase, _animBoolName)
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

        if (enemy.IsPlayerDetected() || enemy.IsPlayerBehindDetected())
            stateMachine.ChangeState(enemy.battleState);
    }

    public override void Exit()
    {
        base.Exit();
    }
}