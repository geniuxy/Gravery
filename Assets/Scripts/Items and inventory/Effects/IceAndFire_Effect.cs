using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Ice_And_Fire_Effect", menuName = "Data/Item Effect/Ice And Fire")]
public class IceAndFire_Effect : ItemEffect
{
    [SerializeField] private GameObject iceAndfirePrefab;
    [SerializeField] private float xVelocity;

    private void OnValidate()
    {
        equipmentEffectDescription = "shooting a ice and fire ball after 3rd attack, with velocity of " + xVelocity;
    }

    public override void ExecuteItemEffect(Transform _respawnTransform)
    {
        Player player = PlayerManager.instance.player;

        bool isThirdAttack = player.primaryAttackState.comboCounter == 2;

        if (isThirdAttack)
        {
            GameObject newThunderStrike =
                Instantiate(iceAndfirePrefab, _respawnTransform.position, player.transform.rotation);
            newThunderStrike.GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity * player.facingDir, 0);
            Destroy(newThunderStrike, 10f);
        }
    }
}