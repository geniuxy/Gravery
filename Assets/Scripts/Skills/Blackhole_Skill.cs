using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Blackhole_Skill : Skill
{
    [Header("CloneAttack info")]
    [SerializeField] private int amountOfCloneAttack = 4;
    [SerializeField] private float cloneAttackCoolDown = .3f;
    [Header("Black hole info")]
    [SerializeField] private UI_SkillSlot skillTreeButton_Blackhole;
    [SerializeField] private GameObject blackholePrefab;
    [SerializeField] private float maxSize = 15f;
    [SerializeField] private float growSpeed = 1f;
    [SerializeField] private float shrinkSpeed = 1f;
    [SerializeField] private float blackholeDuration = 4f;
    public bool blackholeUnlock { get; private set; }

    private Blackhole_Skill_Controller _currentBlackholeController;

    protected override void Update()
    {
        base.Update();
    }

    public override void CheckSkill() => UnlockBlackHole();

    private void UnlockBlackHole() => blackholeUnlock = skillTreeButton_Blackhole.unlock;

    public override void UseSkill()
    {
        base.UseSkill();

        GameObject newBlackHole = Instantiate(blackholePrefab, player.transform.position, Quaternion.identity);

        _currentBlackholeController = newBlackHole.GetComponent<Blackhole_Skill_Controller>();

        _currentBlackholeController.SetupBlackHole(maxSize, growSpeed, shrinkSpeed, amountOfCloneAttack,
            cloneAttackCoolDown, blackholeDuration);

        AudioManager.instance.PlaySFX(7);
    }

    public bool SkillCompleted()
    {
        if (!_currentBlackholeController)
            return false;

        if (_currentBlackholeController.playerCanExitState)
        {
            _currentBlackholeController = null;
            return true;
        }

        return false;
    }

    public float GetBlackHoleRadius() => maxSize / 2;
}