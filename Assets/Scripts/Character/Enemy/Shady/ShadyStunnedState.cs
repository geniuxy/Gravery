using UnityEngine;

public class ShadyStunnedState : EnemyState
{
    private Enemy_Shady enemy;

    public ShadyStunnedState(EnemyStateMachine _stateMachine, Enemy _enemyBase, Enemy_Shady _enemy,
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