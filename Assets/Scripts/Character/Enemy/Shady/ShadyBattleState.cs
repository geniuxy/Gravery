using UnityEngine;

public class ShadyBattleState : EnemyState
{
    private Transform player;
    private Enemy_Shady enemy;

    private int moveDir;
    private float defaultSpeed;

    public ShadyBattleState(EnemyStateMachine _stateMachine, Enemy _enemyBase, Enemy_Shady _enemy,
        string _animBoolName) : base(_stateMachine, _enemyBase, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        defaultSpeed = enemy.moveSpeed;
        enemy.moveSpeed = enemy.battleSpeed;

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
                enemy.stats.DestroyEntity(); // 可以自曝+掉落物品+加钱
        }
        else if (stateTimer < 0 ||
                 Vector2.Distance(player.position, enemy.transform.position) > enemy.battleDistance * 2)
            stateMachine.ChangeState(enemy.idleState);

        // 防止在攻击的时候还移动
        if (enemy.IsPlayerDetected() && enemy.IsPlayerDetected().distance < enemy.attackDistance - .1f)
            return;
        BattleMoveLogic();
    }

    public override void Exit()
    {
        base.Exit();

        enemy.moveSpeed = defaultSpeed;
    }

    private bool CanAttack() => Time.time > enemy.attackCoolDown + enemy.lastTimeAttacked;

    private void BattleMoveLogic()
    {
        if (player.position.x > enemy.transform.position.x)
            moveDir = 1;
        else if (player.position.x < enemy.transform.position.x)
            moveDir = -1;
        enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);
    }
}