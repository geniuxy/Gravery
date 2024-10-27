using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringerSpellCast_Controller : MonoBehaviour
{
    [SerializeField] private Transform checkPos;
    [SerializeField] private Vector2 checkSize;
    [SerializeField] private LayerMask whatIsPlayer;
    private CharacterStats myStats;

    private int spellCastHitDir;

    public void SetSpellCast(CharacterStats _myStats) => myStats = _myStats;

    private void SetSpellCastHitDir(Transform _target) =>
        spellCastHitDir = transform.position.x < _target.position.x ? 1 : -1;

    private void AnimationSpellCastEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(checkPos.position, checkSize, whatIsPlayer);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<PlayerStats>() != null)
            {
                SetSpellCastHitDir(hit.transform);
                myStats.DoMagicDamage(hit.GetComponent<PlayerStats>(), spellCastHitDir);
            }
        }
    }

    private void SelfDestroy() => Destroy(gameObject);

    private void OnDrawGizmos() => Gizmos.DrawWireCube(checkPos.position, checkSize);
}