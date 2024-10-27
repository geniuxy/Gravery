using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Freeze_Enemies_Effect", menuName = "Data/Item Effect/Freeze Enemies")]
public class FreezeEnemies_Effect : ItemEffect
{
    [SerializeField] private float freezeDuration;
    [SerializeField] private float freezeRadius;
    [FormerlySerializedAs("freezeTriggerPercentage")]
    [Range(0, 1f)]
    [SerializeField] private float freezeTriggerHpPercentage;

    private void OnValidate()
    {
        equipmentEffectDescription = "freeze enemies " + freezeDuration + "s in the radius of " + freezeRadius +
                                     " after hp below " + freezeTriggerHpPercentage * 100 + "%";
    }

    public override void ExecuteItemEffect(Transform _transform)
    {
        PlayerStats stats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        if (stats.currentHp > stats.GetMaxHpValue() * freezeTriggerHpPercentage)
            return;

        if (!Inventory.instance.CanUseArmor())
            return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(_transform.position, freezeRadius);

        foreach (var hit in colliders)
            hit.GetComponent<Enemy>()?.FreezeTimeFor(freezeDuration);
    }
}