using UnityEngine;

public class DeathBringerBattleState : EnemyState
{
    private Transform player;
    private Boss_DeathBringer enemy;

    private int moveDir;

    public DeathBringerBattleState(EnemyStateMachine _stateMachine, Enemy _enemyBase, Boss_DeathBringer _enemy,
        string _animBoolName) : base(_stateMachine, _enemyBase, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;
        if (player.GetComponent<PlayerStats>().isDead)
            stateMachine.ChangeState(enemy.moveState);
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerDetected())
        {
            stateTimer = enemy.battleTime;

            if (enemy.IsPlayerDetected().distance < enemy.attackDistance)
                if (CanAttack())
                {
                    enemy.attackCoolDown = Random.Range(enemy.minAttackCoolDown, enemy.maxAttackCoolDown);
                    stateMachine.ChangeState(enemy.attackState);
                }
        }
        else if (stateTimer < 0 ||
                 Vector2.Distance(player.position, enemy.transform.position) > enemy.battleDistance * 2)
            RestoreBossFight();

        // 防止在攻击的时候还移动
        if (enemy.IsPlayerDetected() && enemy.IsPlayerDetected().distance < enemy.attackDistance - .1f)
            return;
        BattleMoveLogic();
    }

    public override void Exit()
    {
        base.Exit();
    }

    private bool CanAttack() => Time.time > enemy.attackCoolDown + enemy.lastTimeAttacked;

    private void RestoreBossFight()
    {
        enemy.isBossFightBegin = false;
        enemy.transform.position = enemy.defaultPosition;
        enemy.stats.currentHp = enemy.stats.GetMaxHpValue();
        enemy.stats.onHealthChanged?.Invoke();
        stateMachine.ChangeState(enemy.idleState);
    }

    private void BattleMoveLogic()
    {
        if (player.position.x > enemy.transform.position.x)
            moveDir = 1;
        else if (player.position.x < enemy.transform.position.x)
            moveDir = -1;
        enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);
    }
}