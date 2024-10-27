using UnityEngine;

public class ArcherBattleState : EnemyState
{
    private Transform player;
    private Enemy_Archer enemy;

    private int moveDir;

    public ArcherBattleState(EnemyStateMachine _stateMachine, Enemy _enemyBase, Enemy_Archer _enemy,
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

            if (enemy.IsPlayerDetected().distance < enemy.saveDistance)
            {
                if (CanJump())
                    stateMachine.ChangeState(enemy.jumpState);
                else if (CanAttack())
                    stateMachine.ChangeState(enemy.meleeAttackState);
            }
            else if (enemy.IsPlayerDetected().distance < enemy.attackDistance)
                if (CanAttack())
                {
                    enemy.attackCoolDown = Random.Range(enemy.minAttackCoolDown, enemy.maxAttackCoolDown);
                    stateMachine.ChangeState(enemy.rangedAttackState);
                }
        }
        else if (stateTimer < 0 ||
                 Vector2.Distance(player.position, enemy.transform.position) > enemy.battleDistance * 2)
            stateMachine.ChangeState(enemy.idleState);

        // 防止在攻击的时候还移动
        // 此处弓箭手没有必要设置移动
        // if (enemy.IsPlayerDetected() && enemy.IsPlayerDetected().distance < enemy.attackDistance - 1f)
        //     return;
        // BattleMoveLogic();
        if ((player.position.x > enemy.transform.position.x && enemy.facingDir == -1) ||
            (player.position.x < enemy.transform.position.x && enemy.facingDir == 1))
            enemy.Flip();
    }

    public override void Exit()
    {
        base.Exit();
    }

    private bool CanAttack() => Time.time > enemy.attackCoolDown + enemy.lastTimeAttacked;

    private bool CanJump()
    {
        if (!enemy.GroundBehind() || enemy.WallBehind())
            return false;
        return Time.time > enemy.jumpCoolDown + enemy.lastTimeJump;
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