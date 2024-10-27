using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class Clone_Skill_Controller : MonoBehaviour
{
    private SpriteRenderer sr;
    private Animator anim;
    private Player player;

    [SerializeField] private float cloneLoosingSpeed;

    private float cloneTimer;
    private Transform closestEnemy = null;
    private int cloneFacingDir = 1;
    private float cloneSearchRadius;
    private bool canDuplicateClone;
    private float duplicateCloneOdds;
    private float attackMultiplication;

    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        player = PlayerManager.instance.player;
    }

    private void Update()
    {
        cloneTimer -= Time.deltaTime;

        if (cloneTimer < 0)
        {
            sr.color = new Color(1, 1, 1, sr.color.a - Time.deltaTime * cloneLoosingSpeed);

            if (sr.color.a <= 0)
                Destroy(gameObject);
        }
    }

    public void SetupClone(Transform _newTransform, float _cloneDuration, bool _canAttack, int _playerFacingDir,
        Vector3 _offset, float _cloneSearchRadius, bool _canDuplicateClone, float _duplicateCloneOdds,
        float _attackMultiplication)
    {
        if (_canAttack)
            anim.SetInteger("AttackNumber", Random.Range(1, 4));

        transform.position = _newTransform.position + _offset;
        cloneTimer = _cloneDuration;
        cloneSearchRadius = _cloneSearchRadius;
        canDuplicateClone = _canDuplicateClone;
        duplicateCloneOdds = _duplicateCloneOdds;
        attackMultiplication = _attackMultiplication;
        SetCloneFacingDir(_playerFacingDir);
    }

    private void AnimationTrigger()
    {
        cloneTimer = -.1f;
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                // 这里应该改成clone攻击
                // player.stats.DoDamage(hit.GetComponent<CharacterStats>(), cloneFacingDir);
                PlayerStats playerStats = player.GetComponent<PlayerStats>();
                EnemyStats enemyStats = hit.GetComponent<EnemyStats>();
                playerStats.CloneDoDamage(enemyStats, cloneFacingDir, attackMultiplication);

                // 如果幻象可继承武器特效，即可执行特效
                if (player.skill.clone.canAttackOnHitEffect)
                    Inventory.instance.GetDevice(EquipmentType.Weapon)?.ExecuteEffect(hit.transform);

                if (canDuplicateClone && Random.Range(0, 100) < duplicateCloneOdds * 100)
                    SkillManager.instance.clone.CreateClone(hit.transform, cloneFacingDir,
                        new Vector3(cloneFacingDir * 1.5f, 0));
            }
        }
    }

    private void SetCloneFacingDir(int _playerFacingDir)
    {
        closestEnemy = SkillManager.instance.clone.SearchClosestEnemy(transform.position, cloneSearchRadius);
        // 如果附近有敌人，幻象面向敌人
        if (closestEnemy != null)
        {
            if (transform.position.x > closestEnemy.position.x)
            {
                transform.Rotate(0, 180, 0);
                cloneFacingDir = -1;
            }
        }
        // 如果附近没有敌人，幻象面向玩家朝向
        else
        {
            if (_playerFacingDir < 0)
            {
                transform.Rotate(0, 180, 0);
                cloneFacingDir = -1;
            }
        }
    }
}