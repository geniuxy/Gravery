using UnityEngine;

public class DeathBringerDeadState : EnemyState
{
    private Boss_DeathBringer enemy;

    public DeathBringerDeadState(EnemyStateMachine _stateMachine, Enemy _enemyBase, Boss_DeathBringer _enemy,
        string _animBoolName) : base(_stateMachine, _enemyBase, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        // 保持死亡前的动画
        enemy.anim.SetBool(enemy.lastAnimBoolName, true);
        enemy.anim.speed = 0;
        enemy.cd.enabled = false;

        stateTimer = .1f;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
            rb.velocity = new Vector2(0, 10);
    }
}