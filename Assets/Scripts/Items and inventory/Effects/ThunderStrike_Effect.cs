using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Thunder_Strike_Effect", menuName = "Data/Item Effect/Thunder Strike")]
public class ThunderStrike_Effect : ItemEffect
{
    [SerializeField] private GameObject thunderStrikePrefab;

    private void OnValidate()
    {
        equipmentEffectDescription = "shooting a thunder strike when attack the shocked enemy";
    }

    public override void ExecuteItemEffect(Transform _enemyTransform)
    {
        if (_enemyTransform.GetComponent<EnemyStats>().isShocked)
        {
            GameObject newThunderStrike =
                Instantiate(thunderStrikePrefab, _enemyTransform.position, Quaternion.identity);
            Destroy(newThunderStrike, 1f);
        }
    }
}