using UnityEngine;

public class ArcherStunnedState : EnemyState
{
    private Enemy_Archer enemy;

    public ArcherStunnedState(EnemyStateMachine _stateMachine, Enemy _enemyBase, Enemy_Archer _enemy,
        string _animBoolName) : base(_stateMachine, _enemyBase, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.fx.PlayDustFX();

        stateTimer = enemy.stunnedDuration;

        rb.velocity = new Vector2(-enemy.facingDir * enemy.stunnedDir.x, enemy.stunnedDir.y);
    }

    public override void Update()
    {
        base.Update();

        enemy.fx.InvokeRepeating("RedColorBlink", 0, .1f);

        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.idleState);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.fx.Invoke("CancelColorChange", 0);
    }
}