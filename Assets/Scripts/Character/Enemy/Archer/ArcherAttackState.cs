using UnityEngine;

public class ArcherAttackState : EnemyState
{
    protected Enemy_Archer enemy;

    public ArcherAttackState(EnemyStateMachine _stateMachine, Enemy _enemyBase, Enemy_Archer _enemy,
        string _animBoolName) : base(_stateMachine, _enemyBase, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        enemy.SetZeroVelocity();

        if (triggerCalled)
            stateMachine.ChangeState(enemy.idleState);
    }

    public override void Exit()
    {
        base.Exit();
        enemy.lastTimeAttacked = Time.time;
    }
}