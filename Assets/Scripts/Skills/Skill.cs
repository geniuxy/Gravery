using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

public class Skill : MonoBehaviour
{
    public float coolDown;
    public float coolDownTimer { get; private set; }

    protected Player player;

    protected virtual void Start()
    {
        player = PlayerManager.instance.player;

        Invoke(nameof(CheckSkill), .1f);
    }

    protected virtual void Update()
    {
        coolDownTimer -= Time.deltaTime;
    }

    public virtual void CheckSkill()
    {
    }

    public virtual bool CanUseSkill()
    {
        if (coolDownTimer < 0)
            return true;

        player.fx.CreatePopUpTextFX("cooling down");
        return false;
    }

    public virtual void UseSkill() => coolDownTimer = coolDown;

    public virtual Transform SearchClosestEnemy(Vector2 _gameObjectPos, float _searchRadius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_gameObjectPos, _searchRadius);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                float enemyDistance = Vector2.Distance(_gameObjectPos, hit.transform.position);

                if (enemyDistance < closestDistance)
                {
                    closestDistance = enemyDistance;
                    closestEnemy = hit.transform;
                }
            }
        }

        return closestEnemy;
    }
}