using UnityEngine;

public class ArcherMeleeAttackState : ArcherAttackState
{
    public ArcherMeleeAttackState(EnemyStateMachine _stateMachine, Enemy _enemyBase, Enemy_Archer _enemy,
        string _animBoolName) : base(_stateMachine, _enemyBase, _enemy, _animBoolName)
    {
    }
}