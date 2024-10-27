using UnityEngine;

public class ShadyDeadState : EnemyState
{
    private Enemy_Shady enemy;

    public ShadyDeadState(EnemyStateMachine _stateMachine, Enemy _enemyBase, Enemy_Shady _enemy,
        string _animBoolName) : base(_stateMachine, _enemyBase, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled)
            enemy.SelfDestroy();
    }
}