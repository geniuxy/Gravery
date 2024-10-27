using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Heal_Effect", menuName = "Data/Item Effect/Heal")]
public class Heal_Effect : ItemEffect
{
    [Range(0, 1f)]
    [SerializeField] private float healingPercentage;

    private void OnValidate()
    {
        equipmentEffectDescription = "+ " + healingPercentage * 100 + "% Hp";
    }

    public override void ExecuteItemEffect(Transform _targetTransform)
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        int healing = Mathf.RoundToInt(playerStats.GetMaxHpValue() * healingPercentage);

        playerStats.IncreaseHealthBy(healing);
    }
}