using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic_Controller : MonoBehaviour
{
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
            EnemyStats enemyStats = collision.GetComponent<EnemyStats>();
            int damageDir = SetDamageDir(collision);
            playerStats.DoMagicDamage(enemyStats, damageDir);
        }
    }

    private int SetDamageDir(Collider2D collision) => transform.position.x < collision.transform.position.x ? 1 : -1;
}