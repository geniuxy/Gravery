using UnityEngine;

public class ShadyIdleState : ShadyGroundedState
{
    public ShadyIdleState(EnemyStateMachine _stateMachine, Enemy _enemyBase, Enemy_Shady _enemy, string _animBoolName) :
        base(_stateMachine, _enemyBase, _enemy, _animBoolName)
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

        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.moveState);
    }

    public override void Exit()
    {
        base.Exit();

        AudioManager.instance.PlaySFX(15, enemy.transform);
    }
}