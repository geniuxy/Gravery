using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive_Controller : MonoBehaviour
{
    private Animator anim;
    private CharacterStats myStats;
    private float growSpeed = 18f;
    private float maxSize = 7f;
    private float explosionRadius;

    private bool canGrow = true;
    private int explosiveHitDir;

    void Update()
    {
        if (canGrow)
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize),
                growSpeed * Time.deltaTime);
        if (maxSize - transform.localScale.x < .2f)
        {
            canGrow = false;
            anim.SetTrigger("Explode");
        }
    }

    public void SetupExplosive(CharacterStats _myStats, float _growSpeed, float _maxSize, float _explosionRadius)
    {
        anim = GetComponent<Animator>();

        myStats = _myStats;
        growSpeed = _growSpeed;
        maxSize = _maxSize;
        explosionRadius = _explosionRadius;
    }

    private void SetExplosiveHitDir(Transform _target) =>
        explosiveHitDir = transform.position.x < _target.position.x ? 1 : -1;

    private void AnimationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<PlayerStats>() != null)
            {
                SetExplosiveHitDir(hit.transform);
                myStats.DoMagicDamage(hit.GetComponent<PlayerStats>(), explosiveHitDir);
            }
        }
    }

    private void SelfDestroy() => Destroy(gameObject);
}