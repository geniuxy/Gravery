using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff_Effect", menuName = "Data/Item Effect/Buff")]
public class Buff_Effect : ItemEffect
{
    private PlayerStats stats;

    [Header("Buff info")]
    [SerializeField] private StatType buffStat;
    [SerializeField] private int buffValue;
    [SerializeField] private float buffDuration;

    private void OnValidate()
    {
        equipmentEffectDescription = "+" + buffValue + " " + buffStat.ToString() + ", duration: " + buffDuration + "s";
    }

    public override void ExecuteItemEffect(Transform _targetTransform)
    {
        stats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        stats.IncreaseStatBy(buffValue, buffDuration, stats.StatOfType(buffStat));
    }
}