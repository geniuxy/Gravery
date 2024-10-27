using UnityEngine;

public class DeathBringerAttackState : EnemyState
{
    private Boss_DeathBringer enemy;

    public DeathBringerAttackState(EnemyStateMachine _stateMachine, Enemy _enemyBase, Boss_DeathBringer _enemy,
        string _animBoolName) : base(_stateMachine, _enemyBase, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.chanceToTeleport += 10f;
    }

    public override void Update()
    {
        base.Update();

        enemy.SetZeroVelocity();

        if (triggerCalled)
        {
            if (enemy.CanTeleport())
                stateMachine.ChangeState(enemy.teleportState); // 攻击完就进入传送
            else
                stateMachine.ChangeState(enemy.battleState); // 攻击完就进入战斗
        }
    }

    public override void Exit()
    {
        base.Exit();
        enemy.lastTimeAttacked = Time.time;
    }
}