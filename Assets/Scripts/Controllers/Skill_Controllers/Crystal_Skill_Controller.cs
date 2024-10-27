using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Crystal_Skill_Controller : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();
    private CircleCollider2D cd => GetComponent<CircleCollider2D>();

    private Player player;

    private float crystalExistTimer;

    private bool canExplode;
    private int crystalHitDir;
    private Transform closestEnemy = null;

    private bool canMove;
    private float moveSpeed;
    private float searchRadius;

    private bool canGrow;
    private float growSpeed;

    private bool crystalInsteadOfClone;
    [SerializeField] private LayerMask whatIsEnemy;

    public void SetupCrystal(float _crystalDuration, bool _canExplode, bool _canMove, float _moveSpeed,
        float _growSpeed, float _searchRadius)
    {
        crystalExistTimer = _crystalDuration;
        canExplode = _canExplode;
        canMove = _canMove;
        moveSpeed = _moveSpeed;
        growSpeed = _growSpeed;
        searchRadius = _searchRadius;
        closestEnemy = SkillManager.instance.crystal.SearchClosestEnemy(transform.position, searchRadius);
    }

    private void Start()
    {
        player = PlayerManager.instance.player;
    }

    private void Update()
    {
        crystalExistTimer -= Time.deltaTime;

        if (canMove && closestEnemy != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, closestEnemy.position,
                moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, closestEnemy.position) < 1)
                ExitCrystal();
        }

        if (!canGrow && crystalExistTimer < 0)
            ExitCrystal();

        if (canGrow)
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(3, 3), growSpeed * Time.deltaTime);
    }

    public void ExitCrystal()
    {
        canMove = false;
        if (canExplode)
        {
            AudioManager.instance.PlaySFX(24, _allowSimultaneousSounds: true);
            canGrow = true;
            anim.SetTrigger("Explode");
        }
        else
            SelfDestroy();
    }

    public void ChooseRandomEnemy()
    {
        float serachRadius = SkillManager.instance.blackhole.GetBlackHoleRadius();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, serachRadius, whatIsEnemy);
        if (colliders.Length > 0)
            closestEnemy = colliders[Random.Range(0, colliders.Length)].transform;
    }

    public void SelfDestroy() => Destroy(gameObject);

    private void SetCrystalHitDir(Transform _enemy) =>
        crystalHitDir = transform.position.x < _enemy.position.x ? 1 : -1;

    private void AnimationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                SetCrystalHitDir(enemy.transform);
                player.stats.DoMagicDamage(enemy.GetComponent<CharacterStats>(), crystalHitDir);
                // 附带物品技能效果
                Inventory.instance.GetDevice(EquipmentType.Amulet)?.ExecuteEffect(enemy.transform);
            }
        }
    }
}