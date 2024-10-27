using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Dash_Skill : Skill
{
    [Header("Unlock dash")]
    [SerializeField] private UI_SkillSlot skillTreeButton_Dash;
    public bool dashUnlock { get; private set; }

    [Header("Unlock clone on dash")]
    [SerializeField] private UI_SkillSlot skillTreeButton_CloneOnDash;
    public bool cloneOnDashUnlock { get; private set; }

    [Header("Unlock clone on arrival")]
    [SerializeField] private UI_SkillSlot skillTreeButton_CloneOnArrival;
    public bool cloneOnArrivalUnlock { get; private set; }

    #region Unlock skill

    public override void CheckSkill()
    {
        UnlockDash();
        UnlockCloneOnDash();
        UnlockCloneOnArrival();
    }

    private void UnlockDash() => dashUnlock = skillTreeButton_Dash.unlock;

    private void UnlockCloneOnDash() => cloneOnDashUnlock = skillTreeButton_CloneOnDash.unlock;

    private void UnlockCloneOnArrival() => cloneOnArrivalUnlock = skillTreeButton_CloneOnArrival.unlock;

    #endregion

    public void cloneOnDash()
    {
        if (cloneOnDashUnlock)
            player.skill.clone.CreateClone(player.transform, player.facingDir, Vector3.zero);
    }

    public void cloneOnArrival()
    {
        if (cloneOnArrivalUnlock)
            player.skill.clone.CreateClone(player.transform, player.facingDir, Vector3.zero);
    }
}