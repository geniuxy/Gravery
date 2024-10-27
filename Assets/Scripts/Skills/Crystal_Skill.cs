using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Crystal_Skill : Skill
{
    [SerializeField] private float crystalDuration;
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;

    private float tmpCoolDown;

    [Header("Crystal info")]
    [SerializeField] private UI_SkillSlot skillTreeButton_Crystal;
    public bool crystalUnlock { get; private set; }

    [Header("Crystal mirage info")]
    [SerializeField] private UI_SkillSlot skillTreeButton_CrystalMirage;
    public bool crystalMirageUnlock { get; private set; }

    [Header("Explode info")]
    [SerializeField] private UI_SkillSlot skillTreeButton_CrystalExplode;
    [SerializeField] private float growSpeed;
    public bool crystalExplodeUnlock { get; private set; }

    [Header("Move info")]
    [SerializeField] private UI_SkillSlot skillTreeButton_CrystalMove;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float searchRadius;
    public bool crystalMoveUnlock { get; private set; }

    [Header("Multi stack info")]
    [SerializeField] private UI_SkillSlot skillTreeButton_MultiCrystal;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCoolDown;
    [SerializeField] private List<GameObject> crystalStack = new List<GameObject>();
    public bool multiCrystalUnlock { get; private set; }

    # region skill unlock

    public override void CheckSkill()
    {
        UnlockCrystal();
        UnlockCrystalMirage();
        UnlockCrystalExplode();
        UnlockCrystalMove();
        UnlockMultiCrystal();
    }

    private void UnlockCrystal() => crystalUnlock = skillTreeButton_Crystal.unlock;

    private void UnlockCrystalMirage() => crystalMirageUnlock = skillTreeButton_CrystalMirage.unlock;

    private void UnlockCrystalExplode() => crystalExplodeUnlock = skillTreeButton_CrystalExplode.unlock;

    private void UnlockCrystalMove() => crystalMoveUnlock = skillTreeButton_CrystalMove.unlock;

    private void UnlockMultiCrystal() => multiCrystalUnlock = skillTreeButton_MultiCrystal.unlock;

    # endregion

    public override bool CanUseSkill() => coolDownTimer < 0;

    public override void UseSkill()
    {
        if (crystalUnlock && !crystalMoveUnlock)
        {
            if (currentCrystal != null)
                CrystalLogic();
            else if (currentCrystal == null && CanUseSkill())
            {
                base.UseSkill();
                CrystalLogic();
            }
            else
                player.fx.CreatePopUpTextFX("cooling down");
        }
        else if (crystalMoveUnlock && !multiCrystalUnlock)
        {
            if (CanUseSkill())
            {
                base.UseSkill();
                CrystalLogic();
            }
            else
                player.fx.CreatePopUpTextFX("cooling down");
        }
        else if (multiCrystalUnlock)
            if ((crystalStack.Count == amountOfStacks && CanUseSkill()) || crystalStack.Count > 0)
            {
                base.UseSkill();
                UseMultiCrystal();
            }
            else
                player.fx.CreatePopUpTextFX("can't use now");
    }

    private void CrystalLogic()
    {
        if (currentCrystal == null)
            CreateCrystal();
        else
        {
            if (crystalMoveUnlock)
                return;

            Vector2 playerPos = player.transform.position;
            // player瞬移到crystal位置
            player.transform.position = currentCrystal.transform.position;
            // crystal瞬移到player位置并爆炸
            currentCrystal.transform.position = playerPos;

            if (crystalMirageUnlock)
            {
                SkillManager.instance.clone.CreateClone(currentCrystal.transform, player.facingDir, Vector3.zero);
                Destroy(currentCrystal);
            }
            else
                currentCrystal.GetComponent<Crystal_Skill_Controller>()?.ExitCrystal();
        }
    }

    private void UseMultiCrystal()
    {
        if (crystalStack.Count <= 0)
            return;

        GameObject crystalToSpawn = crystalStack[crystalStack.Count - 1];
        GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);

        crystalStack.Remove(crystalToSpawn);
        newCrystal.GetComponent<Crystal_Skill_Controller>().SetupCrystal(crystalDuration, crystalExplodeUnlock,
            crystalMoveUnlock, moveSpeed, growSpeed, searchRadius);

        // TODO 实现dota那样子的技能次数充能
        if (crystalStack.Count < amountOfStacks)
            Invoke(nameof(RefillStacks), multiStackCoolDown);
    }

    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);

        Crystal_Skill_Controller currentCrystalController = currentCrystal.GetComponent<Crystal_Skill_Controller>();

        currentCrystalController.SetupCrystal(crystalDuration, crystalExplodeUnlock, crystalMoveUnlock, moveSpeed,
            growSpeed, searchRadius);
    }

    public void CrystalToRandomEnemy() => currentCrystal.GetComponent<Crystal_Skill_Controller>().ChooseRandomEnemy();

    private void RefillStacks() => crystalStack.Add(crystalPrefab);

    public int GetCrystalStackAmount() => crystalStack.Count;
}