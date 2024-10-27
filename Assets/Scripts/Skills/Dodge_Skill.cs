using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dodge_Skill : Skill
{
    [Header("Dodge info")]
    [SerializeField] private UI_SkillSlot skillTreeButton_Dodge;
    [SerializeField] private int evasionModifier;
    public bool dodgeUnlock { get; private set; }

    [Header("Mirage on dodge")]
    [SerializeField] private UI_SkillSlot skillTreeButton_MirageOnDodge;
    public bool mirageOnDodgeUnlock { get; private set; }

    #region Unlock skill

    public override void CheckSkill()
    {
        UnlockDodge();
        UnlockMirageOnDodge();
    }

    private void UnlockDodge()
    {
        if (skillTreeButton_Dodge.unlock && !dodgeUnlock)
        {
            player.stats.evasion.AddModifier(evasionModifier);
            Inventory.instance.UpdataStatsUI();
            dodgeUnlock = true;
        }
    }

    private void UnlockMirageOnDodge() => mirageOnDodgeUnlock = skillTreeButton_MirageOnDodge.unlock;

    #endregion

    public void CreateMirageOnDodge(Transform _transform)
    {
        if (mirageOnDodgeUnlock)
        {
            int enemyFacingDir = _transform.GetComponent<Enemy>().facingDir;
            SkillManager.instance.clone.CreateClone(player.transform, player.facingDir,
                new Vector3(-2 * enemyFacingDir, 0));
        }
    }
}