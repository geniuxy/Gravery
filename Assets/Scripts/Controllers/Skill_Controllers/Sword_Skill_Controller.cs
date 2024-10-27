using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Sword_Skill_Controller : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;

    private bool canRotate = true;
    private bool isReturning;

    [Header("Sword info")]
    private float returnSpeed;
    private int swordHitDir;
    private float frozenTimeDuration;

    [Header("Bounce info")]
    private float bounceSpeed;
    private bool isBouncing;
    private int bounceAmount;
    private List<Transform> enemyTargets;
    private int targetIndex;

    [Header("Pierce info")]
    private int pierceAmount;

    [Header("Spin info")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private float hitCoolDown;
    private float hitTimer;
    private float spinSlideSpeed;
    private bool wasStopped;
    private bool isSpinning;
    private float spinDirection;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        // 让游戏对象(剑)始终面向其移动方向
        if (canRotate)
            transform.right = rb.velocity;

        if (isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position,
                returnSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, player.transform.position) < 1)
                player.CatchTheSword();
        }

        BounceLogic();

        SpinLogic();
    }

    public void ReturnSword()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        // rb.isKinematic = false; => 收物理引擎影响
        transform.parent = null;
        isReturning = true;
    }

    private void DestroyMe()
    {
        SkillManager.instance.sword.UseSkill();
        Destroy(gameObject);
    }

    private int SetupSwordHitDir(Transform enemy) => transform.position.x > enemy.position.x ? -1 : 1;

    private void BounceLogic()
    {
        if (isBouncing && enemyTargets.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTargets[targetIndex].position,
                bounceSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, enemyTargets[targetIndex].position) < .1f)
            {
                Enemy enemy = enemyTargets[targetIndex].GetComponent<Enemy>();
                SwordSkillDamage(enemy);

                targetIndex = (targetIndex + 1) % enemyTargets.Count;
                bounceAmount--;
                if (bounceAmount <= 0)
                {
                    isBouncing = false;
                    isReturning = true;
                }
            }
        }
    }

    private void SpinLogic()
    {
        if (isSpinning)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
            {
                StopWhenSpinning();
            }

            if (wasStopped)
            {
                // 在剑悬停时仍可向扔出方向移动
                transform.position = Vector2.MoveTowards(transform.position,
                    new Vector2(transform.position.x + spinDirection, transform.position.y), 1.5f * Time.deltaTime);

                spinTimer -= Time.deltaTime;

                if (spinTimer < 0)
                {
                    // wasStopped = false; 这里不需要设置wasStopped = false 因为剑回归了就是一次性的了 会重置参数
                    isSpinning = false;
                    isReturning = true;
                }

                hitTimer -= Time.deltaTime;
                if (hitTimer < 0)
                {
                    hitTimer = hitCoolDown;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);

                    foreach (var hit in colliders)
                    {
                        if (hit.GetComponent<Enemy>() != null)
                        {
                            Enemy enemy = hit.GetComponent<Enemy>();
                            SwordSkillDamage(enemy);
                        }
                    }
                }
            }
        }
    }

    private void StopWhenSpinning()
    {
        if (!wasStopped)
            spinTimer = spinDuration;
        wasStopped = true;
        // 使剑停在原地
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
    }

    public void SetupSword(Vector2 _dir, float _gravityScale, Player _player, float _frozenTimeDuration,
        float _returnSpeed)
    {
        player = _player;
        rb.velocity = _dir;
        rb.gravityScale = _gravityScale;
        frozenTimeDuration = _frozenTimeDuration;
        returnSpeed = _returnSpeed;

        // 在pierceAmount为0的情况下，才需要剑旋转
        if (pierceAmount <= 0)
            anim.SetBool("Rotation", true);

        // Mathf.Clamp() 限制函数
        // rb.velocity.x < a  => spinDirection = a
        // a < rb.velocity.x < b => spinDirection = rb.velocity.x
        // rb.velocity.x > b   => spinDirection = b
        spinDirection = Mathf.Clamp(rb.velocity.x, -spinSlideSpeed, spinSlideSpeed);

        Invoke(nameof(DestroyMe), 7);
    }

    public void SetupBounce(bool _isBouncing, int _bounceAmount, float _bounceSpeed)
    {
        isBouncing = _isBouncing;
        bounceAmount = _bounceAmount;
        bounceSpeed = _bounceSpeed;
        enemyTargets = new List<Transform>();
    }

    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;
    }

    public void SetupSpin(bool _isSpinning, float _maxTravelDistance, float _spinDuration, float _hitCoolDown,
        float _spinSlideSpeed)
    {
        isSpinning = _isSpinning;
        maxTravelDistance = _maxTravelDistance;
        spinDuration = _spinDuration;
        hitCoolDown = _hitCoolDown;
        spinSlideSpeed = _spinSlideSpeed;
    }

    // 勾选上了collider的isTrigger字段就可以触发这些函数
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 这一步是为了在收回剑的路途中剑的状态不会受敌人影响
        if (isReturning)
            return;

        // 对剑攻击经过的敌人造成伤害
        if (collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            SwordSkillDamage(enemy);
        }

        SetupTargetsForBounce(collision);

        StuckInto(collision);
    }

    private void SwordSkillDamage(Enemy enemy)
    {
        swordHitDir = SetupSwordHitDir(enemy.transform);
        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
        player.stats.DoDamage(enemyStats, swordHitDir);
        // 冻结敌人动作一段时间
        if (player.skill.sword.timeStopUnlock)
            enemy.FreezeTimeFor(frozenTimeDuration);
        // 进一步解锁技能，可以增大敌人承伤
        if (player.skill.sword.vulnerableUnlock)
            enemyStats.MakeVulnerableFor(frozenTimeDuration);
        // 附带物品技能效果
        Inventory.instance.GetDevice(EquipmentType.Amulet)?.ExecuteEffect(enemy.transform);

        AudioManager.instance.PlaySFX(25, _allowSimultaneousSounds: true);
    }

    private void SetupTargetsForBounce(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            if (isBouncing && enemyTargets.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);

                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                        enemyTargets.Add(hit.transform);
                }
            }
        }
    }

    private void StuckInto(Collider2D collision)
    {
        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount--;
            return;
        }

        if (isSpinning)
        {
            StopWhenSpinning();
            return;
        }

        canRotate = false;
        cd.enabled = false;

        // 使rb不受物理引擎影响
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        GetComponentInChildren<ParticleSystem>()?.Play();

        // 如果剑正在弹跳，就保持旋转且不随ground或敌人运动
        if (isBouncing && enemyTargets.Count > 0)
            return;

        anim.SetBool("Rotation", false);
        // 使物体跟随碰撞体运动
        transform.parent = collision.transform;
    }
}