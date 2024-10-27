using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Armor,
    Amulet,
    Flask
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Equipment")]
public class ItemData_Equipment : ItemData
{
    public EquipmentType equipmentType;
    public int descriptionLength;
    public float coolDown;

    [Header("Effects info")]
    public ItemEffect[] equipmentEffects;

    [Header("Major stats")]
    public int strength; // 力量：提升攻击和暴击伤害
    public int agility; // 敏捷：提升闪避和暴击几率
    public int intelligence; // 智力：提升1点魔法伤害和3点魔法抵抗
    public int vitality; // 活力：提升5点生命值

    [Header("Offensive stats")]
    public int damage;
    public int critChance;
    public int critPower;

    [Header("Defensive stats")]
    public int maxHp;
    public int armor; // 基本护甲
    public int evasion; // 基本闪避
    public int magicResistance; // 魔法抗性

    [Header("Magic stats")]
    public int fireDamage;
    public int iceDamage;
    public int lightingDamage;

    [Header("Craft materials")]
    public List<InventoryItem> craftMaterials;

    public void AddModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.AddModifier(strength);
        playerStats.agility.AddModifier(agility);
        playerStats.intelligence.AddModifier(intelligence);
        playerStats.vitality.AddModifier(vitality);

        playerStats.damage.AddModifier(damage);
        playerStats.critChance.AddModifier(critChance);
        playerStats.critPower.AddModifier(critPower);

        playerStats.maxHp.AddModifier(maxHp);
        playerStats.armor.AddModifier(armor);
        playerStats.evasion.AddModifier(evasion);
        playerStats.magicResistance.AddModifier(magicResistance);

        playerStats.fireDamage.AddModifier(fireDamage);
        playerStats.iceDamage.AddModifier(iceDamage);
        playerStats.lightingDamage.AddModifier(lightingDamage);
    }

    public void RemoveModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.RemoveModifier(strength);
        playerStats.agility.RemoveModifier(agility);
        playerStats.intelligence.RemoveModifier(intelligence);
        playerStats.vitality.RemoveModifier(vitality);

        playerStats.damage.RemoveModifier(damage);
        playerStats.critChance.RemoveModifier(critChance);
        playerStats.critPower.RemoveModifier(critPower);

        playerStats.maxHp.RemoveModifier(maxHp);
        playerStats.armor.RemoveModifier(armor);
        playerStats.evasion.RemoveModifier(evasion);
        playerStats.magicResistance.RemoveModifier(magicResistance);

        playerStats.fireDamage.RemoveModifier(fireDamage);
        playerStats.iceDamage.RemoveModifier(iceDamage);
        playerStats.lightingDamage.RemoveModifier(lightingDamage);
    }

    public void ExecuteEffect(Transform _enemyTransform)
    {
        foreach (ItemEffect equipmentEffect in equipmentEffects)
            equipmentEffect.ExecuteItemEffect(_enemyTransform);
    }

    public override string GetItemDescription()
    {
        sb.Clear();
        descriptionLength = 0;

        AddDescription("Strength", strength);
        AddDescription("Agility", agility);
        AddDescription("Intelligence", intelligence);
        AddDescription("Vitality", vitality);

        AddDescription("Damage", damage);
        AddDescription("Crit Chance", critChance);
        AddDescription("Crit Power", critPower);

        AddDescription("Max HP", maxHp);
        AddDescription("Armor", armor);
        AddDescription("Evasion", evasion);
        AddDescription("Magic Resistance", magicResistance);

        if (equipmentEffects != null)
        {
            for (int i = 0; i < equipmentEffects.Length; i++)
            {
                sb.AppendLine();
                sb.Append("Unique " + (i + 1) + ": ");
                sb.Append(equipmentEffects[i].equipmentEffectDescription);
                descriptionLength++;
            }
        }

        if (descriptionLength < 5)
        {
            for (int i = 0; i < 5 - descriptionLength; i++)
            {
                sb.Append(" ");
                sb.Append("\n");
            }
        }

        return sb.ToString();
    }

    public void AddDescription(string _name, int _value)
    {
        if (_value > 0)
        {
            sb.Append("+" + _name + ": " + _value);
            sb.AppendLine();
            descriptionLength++;
        }
    }
}