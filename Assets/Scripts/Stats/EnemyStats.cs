using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    private Enemy enemy;

    [Header("Drop System")]
    public Stat dropCurrency;

    [Header("Level details")]
    [SerializeField] private int level = 1;
    [Range(0f, 1f)]
    [SerializeField] private float percentageModifier = .4f;

    protected override void Start()
    {
        // 敌人每级都会涨属性
        // 这里最大血量在start执行之前计算
        // 然后复制给currentHp
        dropCurrency.SetDefaultValue(100);
        ApplyLevelModifier();
        base.Start();
        currentHp = GetMaxHpValue();
        enemy = GetComponent<Enemy>();
        dropSystem = GetComponent<ItemDrop>();
    }

    private void ApplyLevelModifier()
    {
        Modify(strength);
        Modify(agility);
        Modify(intelligence);
        Modify(vitality);

        Modify(damage);
        Modify(critChance);
        Modify(critPower);

        Modify(maxHp);
        Modify(armor);
        Modify(evasion);
        Modify(magicResistance);

        Modify(fireDamage);
        Modify(iceDamage);
        Modify(lightingDamage);

        Modify(dropCurrency);
    }

    private void Modify(Stat _stat)
    {
        for (int i = 1; i < level; i++)
        {
            float modifier = _stat.GetValue() * percentageModifier;

            _stat.AddModifier(Mathf.RoundToInt(modifier));
        }
    }

    protected override void Die()
    {
        base.Die();
        enemy.Die();

        PlayerManager.instance.currency += dropCurrency.GetValue();
        dropSystem.GenerateDrop();
        Destroy(gameObject, 5f);
    }
}