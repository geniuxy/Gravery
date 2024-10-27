using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private Player player;

    protected override void Start()
    {
        base.Start();
        currentHp = GetMaxHpValue();
        onHealthChanged?.Invoke();
        player = GetComponent<Player>();
        dropSystem = GetComponent<PlayerItemDrop>();
    }

    protected override void Die()
    {
        base.Die();

        DecreaseCurrency();

        player.Die();
        dropSystem.GenerateDrop();
    }

    private void DecreaseCurrency()
    {
        GameManager.instance.lostCurrencyAmount =
            Mathf.RoundToInt(PlayerManager.instance.currency * GameManager.instance.lostPercentage);
        PlayerManager.instance.currency -= GameManager.instance.lostCurrencyAmount;
    }

    protected override void DecreaseHealthBy(int _damage)
    {
        base.DecreaseHealthBy(_damage);

        if (_damage > GetMaxHpValue() * .3f)
        {
            player.fx.ScreenShock(player.fx.highDamageShockPower);
            player.SetupKnockbackDir(new Vector2(8, 10));
            int RandomIdx = Random.Range(16, 19);
            AudioManager.instance.PlaySFX(RandomIdx);
        }

        ItemData_Equipment armorData = Inventory.instance.GetDevice(EquipmentType.Armor);
        if (armorData != null && armorData.equipmentEffects.Length > 0)
            armorData.ExecuteEffect(transform);
    }

    public override void OnEvasion(Transform _targetTransform)
    {
        base.OnEvasion(_targetTransform);
        SkillManager.instance.dodge.CreateMirageOnDodge(_targetTransform);
    }

    public void CloneDoDamage(CharacterStats _targetStats, int _damageDir, float _multiplication)
    {
        // 是否能闪避
        if (CanTargetAvoidAttack(_targetStats))
            return;

        int totalDamage = _multiplication > 0
            ? Mathf.RoundToInt((damage.GetValue() + strength.GetValue()) * _multiplication)
            : 0;

        // 是否暴击
        bool canCrit = CanCrit();
        if (canCrit)
            totalDamage = CalculateCritDamage(totalDamage);

        // 计算目标护甲
        totalDamage = CheckTargetArmor(_targetStats, totalDamage);

        _targetStats.TakeDamage(totalDamage, canCrit, _damageDir);

        // TODO 如果武器带元素伤害，便施加魔法伤害
        DoMagicDamage(_targetStats, _damageDir);
    }
}