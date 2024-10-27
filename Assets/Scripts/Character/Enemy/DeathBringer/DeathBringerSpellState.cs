using UnityEngine;

public class DeathBringerSpellState : EnemyState
{
    private Boss_DeathBringer enemy;
    private int amountOfSpells;
    private float spellCastTimer;

    public DeathBringerSpellState(EnemyStateMachine _stateMachine, Enemy _enemyBase, Boss_DeathBringer _enemy,
        string _animBoolName) : base(_stateMachine, _enemyBase, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        amountOfSpells = enemy.amountOfSpells;
        spellCastTimer = .5f;
        stateTimer = 6f;
    }

    public override void Update()
    {
        base.Update();

        spellCastTimer -= Time.deltaTime;

        if (CanSpellCast())
            enemy.CreateSpellCast();
        else if (amountOfSpells <= 0 || stateTimer < 0)
            stateMachine.ChangeState(enemy.teleportState);
    }

    public override void Exit()
    {
        base.Exit();
        enemy.lastTimeSpellState = Time.time;
    }

    private bool CanSpellCast()
    {
        if (amountOfSpells > 0 && spellCastTimer <= 0)
        {
            spellCastTimer = Random.Range(enemy.minSpellCastCoolDown, enemy.maxSpellCastCoolDown);
            amountOfSpells--;
            return true;
        }

        return false;
    }
}