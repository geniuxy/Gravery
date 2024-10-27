using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    strength,
    agility,
    intelligence,
    vitality,
    damage,
    critChance,
    critPower,
    maxHp,
    armor,
    evasion,
    magicResistance,
    fireDamage,
    iceDamage,
    lightingDamage
}

public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;
    protected ItemDrop dropSystem;

    [Header("Major stats")]
    public Stat strength; // 力量：提升攻击和暴击伤害
    public Stat agility; // 敏捷：提升闪避和暴击几率
    public Stat intelligence; // 智力：提升1点魔法伤害和3点魔法抵抗
    public Stat vitality; // 活力：提升5点生命值

    [Header("Offensive stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower;

    [Header("Defensive stats")]
    public Stat maxHp;
    public Stat armor; // 基本护甲
    public Stat evasion; // 基本闪避
    public Stat magicResistance; // 魔法抗性

    [Header("Magic stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightingDamage;

    public bool isIgnited; // 持续伤害
    public bool isChilled; // 降低20%护甲
    public bool isShocked; // 降低20%命中率

    [SerializeField] private float ailmentsDuration;
    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;

    private float ignitedDamageCoolDown = .3f;
    private float ignitedDamageTimer;
    private int igniteDamage;
    [SerializeField] private GameObject shockStrikePrefab;
    private int shockDamage;

    [Space]
    public int currentHp;

    public bool isDead { get; private set; }
    private bool isInvincible; // 无敌的
    private bool isVulnerable; // 有破绽的

    // 委托是一种引用类型，用于封装可以被调用的方法
    public System.Action onHealthChanged;

    protected virtual void Start()
    {
        fx = GetComponent<EntityFX>();

        critPower.SetDefaultValue(150);
    }

    protected virtual void Update()
    {
        if (currentHp > GetMaxHpValue())
            currentHp = GetMaxHpValue();

        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;
        ignitedDamageTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
            isIgnited = false;

        if (chilledTimer < 0)
            isChilled = false;

        if (shockedTimer < 0)
            isShocked = false;

        if (isIgnited)
            ApplyIgniteDamage();
    }

    public void IncreaseStatBy(int _modifier, float _duration, Stat _statToModify) =>
        StartCoroutine(StatModCoroutine(_modifier, _duration, _statToModify));

    private IEnumerator StatModCoroutine(int _modifier, float _duration, Stat _statToModify)
    {
        _statToModify.AddModifier(_modifier);
        yield return new WaitForSeconds(_duration);
        _statToModify.RemoveModifier(_modifier);
    }

    public Stat StatOfType(StatType _stat)
    {
        if (_stat == StatType.strength) return strength;
        if (_stat == StatType.agility) return agility;
        if (_stat == StatType.intelligence) return intelligence;
        if (_stat == StatType.vitality) return vitality;
        if (_stat == StatType.damage) return damage;
        if (_stat == StatType.critChance) return critChance;
        if (_stat == StatType.critPower) return critPower;
        if (_stat == StatType.maxHp) return maxHp;
        if (_stat == StatType.armor) return armor;
        if (_stat == StatType.evasion) return evasion;
        if (_stat == StatType.magicResistance) return magicResistance;
        if (_stat == StatType.fireDamage) return fireDamage;
        if (_stat == StatType.iceDamage) return iceDamage;
        if (_stat == StatType.lightingDamage) return lightingDamage;
        return null;
    }

    public void MakeVulnerableFor(float _duration) => StartCoroutine(VulnerableCoroutine(_duration));

    private IEnumerator VulnerableCoroutine(float _duration)
    {
        isVulnerable = true;
        yield return new WaitForSeconds(_duration);
        isVulnerable = false;
    }

    public virtual void DoDamage(CharacterStats _targetStats, int _damageDir)
    {
        // 是否能闪避
        if (CanTargetAvoidAttack(_targetStats))
            return;

        if (_targetStats.isInvincible)
            return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        bool canCrit = CanCrit();
        // 是否暴击
        if (canCrit)
            totalDamage = CalculateCritDamage(totalDamage);

        fx.CreateHitFX(_targetStats.transform, canCrit);

        // 计算目标护甲
        totalDamage = CheckTargetArmor(_targetStats, totalDamage);

        _targetStats.TakeDamage(totalDamage, canCrit, _damageDir);

        // TODO 如果武器带元素伤害，便施加魔法伤害
        // DoMagicDamage(_targetStats, _damageDir);
    }

    public virtual void TakeDamage(int _damage, bool canCrit = false, int damageDir = 0)
    {
        if (isDead)
            return;

        DecreaseHealthBy(_damage);

        if (_damage > 0)
            fx.CreatePopUpTextFX(_damage.ToString(), canCrit ? Color.green : Color.white);

        GetComponent<Entity>().DamageImpact(damageDir);

        fx.StartCoroutine("FlashFX");

        if (currentHp < 0 && !isDead)
            Die();
    }

    public virtual void IncreaseHealthBy(int _healing)
    {
        currentHp = currentHp > GetMaxHpValue() ? GetMaxHpValue() : currentHp + _healing;

        onHealthChanged?.Invoke();
    }

    protected virtual void DecreaseHealthBy(int _damage)
    {
        // 如果处于vulnerable状态承伤为1.1倍
        currentHp -= isVulnerable ? Mathf.RoundToInt(_damage * 1.1f) : _damage;

        onHealthChanged?.Invoke();
    }

    protected virtual void Die() => isDead = true;

    public void DestroyEntity()
    {
        if (!isDead)
            Die();
    }

    public void MakeInvincible(bool _isInvincible) => isInvincible = _isInvincible;

    #region Magical damage and ailments

    public virtual void DoMagicDamage(CharacterStats _targetStats, int _damageDir)
    {
        if (_targetStats.isInvincible)
            return;

        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightingDamage.GetValue();

        int totalMagicDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue();

        totalMagicDamage = CheckTargetResistance(_targetStats, totalMagicDamage);
        _targetStats.TakeDamage(totalMagicDamage, damageDir: _damageDir);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0)
            return;

        AttemptToApplyAilments(_targetStats, _fireDamage, _iceDamage, _lightingDamage);
    }

    private void AttemptToApplyAilments(CharacterStats _targetStats, int _fireDamage, int _iceDamage,
        int _lightingDamage)
    {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightingDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightingDamage;
        bool canApplyShock = _lightingDamage > _fireDamage && _lightingDamage > _iceDamage;

        while (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if (Random.value < .33f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                break;
            }

            if (Random.value < .5f && _iceDamage > 0)
            {
                canApplyChill = true;
                break;
            }

            if (Random.value < 1f && _lightingDamage > 0)
            {
                canApplyShock = true;
                break;
            }
        }

        if (canApplyIgnite)
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));

        if (canApplyShock)
            _targetStats.SetupShockDamage(Mathf.RoundToInt(_lightingDamage * .2f));

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    public void ApplyAilments(bool _ignited, bool _chilled, bool _shocked)
    {
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        // 可以被shock，在被shock了之后仍然可以进行操作。所以，这里没有“!isShocked”
        bool canApplyShock = !isIgnited && !isChilled;

        if (_ignited && canApplyIgnite)
        {
            isIgnited = _ignited;
            ignitedTimer = ailmentsDuration;

            fx.IgniteFxFor(ailmentsDuration);
        }

        if (_chilled && canApplyChill)
        {
            isChilled = _chilled;
            chilledTimer = ailmentsDuration;

            float slowPercentage = .2f;
            GetComponent<Entity>().SlowEntityBy(slowPercentage, ailmentsDuration);
            fx.ChillFxFor(ailmentsDuration);
        }

        if (_shocked && canApplyShock)
        {
            if (!isShocked)
                ApplyShock(_shocked);
            else
            {
                //TODO 这边以后可以做成一个装备是否可以反弹闪电
                if (GetComponent<Player>() != null)
                    return;

                HitClosestEnemyWithShockStrike();
            }
        }
    }

    private void ApplyIgniteDamage()
    {
        if (ignitedDamageTimer < 0)
        {
            DecreaseHealthBy(igniteDamage);
            fx.CreatePopUpTextFX(igniteDamage.ToString(), Color.red);

            if (currentHp < 0 && !isDead)
                Die();

            ignitedDamageTimer = ignitedDamageCoolDown;
        }
    }

    public void ApplyShock(bool _shocked)
    {
        if (isShocked)
            return;
        isShocked = _shocked;
        shockedTimer = ailmentsDuration;
        fx.ShockFxFor(ailmentsDuration);
    }

    private void HitClosestEnemyWithShockStrike()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Transform>() == transform) // 如果是敌人自身就跳过
                continue;
            if (hit.GetComponent<Enemy>() != null)
            {
                float enemyDistance = Vector2.Distance(transform.position, hit.transform.position);

                if (enemyDistance < closestDistance)
                {
                    closestDistance = enemyDistance;
                    closestEnemy = hit.transform;
                }
            }
        }

        if (closestEnemy == null)
            closestEnemy = transform;

        GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
        newShockStrike.GetComponent<ShockStrike_Controller>()
            .Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());
    }

    private void SetupIgniteDamage(int _damage) => igniteDamage = _damage;

    private void SetupShockDamage(int _damage) => shockDamage = _damage;

    #endregion

    # region Stat calculations

    protected int CheckTargetArmor(CharacterStats _stats, int totalDamage)
    {
        if (isChilled)
            totalDamage -= Mathf.RoundToInt(_stats.armor.GetValue() * .8f);
        else
            totalDamage -= _stats.armor.GetValue();
        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }

    private int CheckTargetResistance(CharacterStats _targetStats, int totalMagicDamage)
    {
        totalMagicDamage -= _targetStats.intelligence.GetValue() * 3 + _targetStats.magicResistance.GetValue();
        totalMagicDamage = Mathf.Clamp(totalMagicDamage, 0, int.MaxValue);
        return totalMagicDamage;
    }

    public virtual void OnEvasion(Transform _targetTransform) =>
        AudioManager.instance.PlaySFX(20, _allowSimultaneousSounds: true);

    protected bool CanTargetAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();
        if (isShocked)
            totalEvasion += 20;

        if (Random.Range(0, 100) < totalEvasion)
        {
            _targetStats.OnEvasion(transform);
            return true;
        }

        return false;
    }

    protected bool CanCrit()
    {
        int totalCritChance = critChance.GetValue() + agility.GetValue();

        return Random.Range(0, 100) < totalCritChance;
    }

    protected int CalculateCritDamage(int _damage)
    {
        float totalCritPower = (strength.GetValue() + critPower.GetValue()) * .01f;
        float critDamage = _damage * totalCritPower;

        return Mathf.RoundToInt(critDamage);
    }

    public int GetMaxHpValue() => maxHp.GetValue() + vitality.GetValue() * 5;

    #endregion
}