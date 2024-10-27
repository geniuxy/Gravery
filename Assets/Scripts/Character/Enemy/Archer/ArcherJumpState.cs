using UnityEngine;

public class ArcherJumpState : EnemyState
{
    private Enemy_Archer enemy;

    public ArcherJumpState(EnemyStateMachine _stateMachine, Enemy _enemyBase, Enemy_Archer _enemy, string _animBoolName)
        : base(_stateMachine, _enemyBase, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        rb.velocity = new Vector2(enemy.jumpForce.x * -enemy.facingDir, enemy.jumpForce.y);
    }

    public override void Update()
    {
        base.Update();

        enemy.anim.SetFloat("yVelocity", rb.velocity.y);

        if (rb.velocity.y < 0 && enemy.IsGroundDetected())
            stateMachine.ChangeState(enemy.battleState);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.lastTimeJump = Time.time;
    }
}