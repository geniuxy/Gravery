using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clone_Skill : Skill
{
    [Header("Clone info")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [SerializeField] private float cloneSearchRadius;
    private float attackMultiplication;

    [Header("Clone attack")]
    [SerializeField] private UI_SkillSlot skillTreeButton_CloneAttack;
    [SerializeField] private float cloneAttackMultiplication;
    public bool canAttack { get; private set; }

    [Header("Aggressive clone")]
    [SerializeField] private UI_SkillSlot skillTreeButton_AggressiveClone;
    [SerializeField] private float aggressiveCloneMultiplication;
    public bool canAttackOnHitEffect { get; private set; }

    [Header("Duplicate clone info")]
    [SerializeField] private UI_SkillSlot skillTreeButton_DuplicateClone;
    [SerializeField] private float duplicateCloneMultiplication;
    [SerializeField] private float duplicateCloneOdds;
    public bool canDuplicateClone { get; private set; }

    [Header("Crystal instead of clone")]
    [SerializeField] private UI_SkillSlot skillTreeButton_CrystalMirage;
    public bool crystalInsteadOfClone { get; private set; }

    # region Unlock skill

    public override void CheckSkill()
    {
        UnlockCloneAttack();
        UnlockAggressiveClone();
        UnlockDuplicationClone();
        UnlockCrystalMirage();
    }

    private void UnlockCloneAttack()
    {
        if (!skillTreeButton_CloneAttack.unlock) return;
        canAttack = true;
        attackMultiplication = cloneAttackMultiplication;
    }

    private void UnlockAggressiveClone()
    {
        if (!skillTreeButton_AggressiveClone.unlock) return;
        canAttackOnHitEffect = true;
        attackMultiplication = aggressiveCloneMultiplication;
    }

    private void UnlockDuplicationClone()
    {
        if (!skillTreeButton_DuplicateClone.unlock) return;
        canDuplicateClone = true;
        attackMultiplication = duplicateCloneMultiplication;
    }

    private void UnlockCrystalMirage() => crystalInsteadOfClone = skillTreeButton_CrystalMirage.unlock;

    # endregion

    public void CreateClone(Transform _cloneTransform, int _playerFacingDir, Vector3 _offset)
    {
        if (crystalInsteadOfClone)
        {
            SkillManager.instance.crystal.CreateCrystal();
            return;
        }

        GameObject newClone = Instantiate(clonePrefab);

        newClone.GetComponent<Clone_Skill_Controller>().SetupClone(_cloneTransform, cloneDuration, canAttack,
            _playerFacingDir, _offset, cloneSearchRadius, canDuplicateClone, duplicateCloneOdds, attackMultiplication);
    }

    public void CreateCloneOnParry(Transform _cloneTransform, int _playerFacingDir, Vector3 _offset) =>
        StartCoroutine(CloneDelayCoroutine(_cloneTransform, _playerFacingDir, _offset, .5f));

    private IEnumerator CloneDelayCoroutine(Transform _cloneTransform, int _playerFacingDir, Vector3 _offset,
        float _seconds)
    {
        yield return new WaitForSeconds(_seconds);
        CreateClone(_cloneTransform, _playerFacingDir, _offset);
    }
}