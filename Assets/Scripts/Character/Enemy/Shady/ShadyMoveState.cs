using UnityEngine;

public class ShadyMoveState : ShadyGroundedState
{
    public ShadyMoveState(EnemyStateMachine _stateMachine, Enemy _enemyBase, Enemy_Shady _enemy, string _animBoolName) :
        base(_stateMachine, _enemyBase, _enemy, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        enemy.SetVelocity(enemy.moveSpeed * enemy.facingDir, rb.velocity.y);
        if (!enemy.IsGroundDetected() || enemy.IsWallDetected())
        {
            enemy.Flip();
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}