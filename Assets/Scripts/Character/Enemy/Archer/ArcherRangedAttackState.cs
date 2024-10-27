using UnityEngine;

public class ArcherRangedAttackState : ArcherAttackState
{
    public ArcherRangedAttackState(EnemyStateMachine _stateMachine, Enemy _enemyBase, Enemy_Archer _enemy,
        string _animBoolName) : base(_stateMachine, _enemyBase, _enemy, _animBoolName)
    {
    }
}