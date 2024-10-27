using UnityEngine;

public class ShadyGroundedState : EnemyState
{
    protected Enemy_Shady enemy;
    protected Transform player;

    public ShadyGroundedState(EnemyStateMachine _stateMachine, Enemy _enemyBase, Enemy_Shady _enemy,
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