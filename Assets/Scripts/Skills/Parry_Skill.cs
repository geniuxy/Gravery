using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Parry_Skill : Skill
{
    [Header("Parry")]
    [SerializeField] private UI_SkillSlot skillTreeButton_parry;
    public bool parryUnlock { get; private set; }

    [Header("Parry with restore")]
    [Range(0f, 1f)]
    [SerializeField] private float restoreHpPercentage;
    [SerializeField] private UI_SkillSlot skillTreeButton_parryWithRestore;
    public bool parryWithRestoreUnlock { get; private set; }

    [Header("Parry with Mirage")]
    [SerializeField] private UI_SkillSlot skillTreeButton_parryWithMirage;
    public bool parryWithMirageUnlock { get; private set; }

    public override void UseSkill()
    {
        base.UseSkill();
    }

    #region Unlock skill

    public override void CheckSkill()
    {
        UnlockParry();
        UnlockParryWithRestore();
        UnlockParryWithMirage();
    }

    private void UnlockParry() =>
        parryUnlock = skillTreeButton_parry.unlock;

    private void UnlockParryWithRestore() =>
        parryWithRestoreUnlock = skillTreeButton_parryWithRestore.unlock;

    private void UnlockParryWithMirage() =>
        parryWithMirageUnlock = skillTreeButton_parryWithMirage.unlock;

    #endregion

    public void MakeMirageOnParry(Transform _targetTransform)
    {
        if (parryWithMirageUnlock)
            SkillManager.instance.clone.CreateCloneOnParry(_targetTransform, player.facingDir,
                new Vector3(player.facingDir * 2, 0));
    }
}