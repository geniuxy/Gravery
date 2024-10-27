using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI_SkillSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ISaveManager
{
    public bool unlock;
    [SerializeField] private string skillName;
    [SerializeField] private int skillCost;
    [SerializeField] private Color lockedSkillColor;
    [TextArea]
    [SerializeField] private string skillDescription;
    [SerializeField] private UI_SkillSlot[] preSkills;
    [SerializeField] private UI_SkillSlot[] conflictSkills;
    [SerializeField] private UI_SkillSlot[] postSkills;

    private Image skillImage;

    private void OnValidate() => gameObject.name = "SkillSlot - " + skillName;

    private void Awake() => InitSkillImage();

    private void InitSkillImage()
    {
        skillImage = GetComponent<Image>();
        skillImage.color = lockedSkillColor;

        if (unlock)
            skillImage.color = Color.white;
    }

    public void LoadData(GameData _data)
    {
        if (_data.skillTree.TryGetValue(skillName, out bool value))
            unlock = value;
    }

    public void SaveData(ref GameData _data)
    {
        // 先删除该技能信息，防止字典key重复
        _data.skillTree.Remove(skillName);

        _data.skillTree.Add(skillName, unlock);
    }

    public void OnPointerEnter(PointerEventData eventData) =>
        UI.instance.ui_SkillTooltip.ShowTooltip(skillName, skillDescription, skillCost);

    public void OnPointerExit(PointerEventData eventData) =>
        UI.instance.ui_SkillTooltip.HideTooltip();

    public void OnPointerClick(PointerEventData eventData)
    {
        // 检查鼠标按钮
        if (eventData.button == PointerEventData.InputButton.Left)
            OnLeftClick();
        else if (eventData.button == PointerEventData.InputButton.Right)
            OnRightClick();
        CheckAllSkillUnlock();
    }

    private void OnLeftClick() => UnlockSkillSlot();

    private void OnRightClick() => LockSkillSlot();

    private void CheckAllSkillUnlock()
    {
        foreach (Skill skill in SkillManager.instance.GetComponentsInChildren<Skill>())
            skill.CheckSkill();
    }

    private void UnlockSkillSlot()
    {
        foreach (var preSkill in preSkills)
            if (!preSkill.unlock)
            {
                Debug.Log("the pre skill" + preSkill.name + " is not unlocked");
                return;
            }

        foreach (var conflictSkill in conflictSkills)
            if (conflictSkill.unlock)
            {
                Debug.Log("the conflict skill" + conflictSkill.name + " is unlocked");
                return;
            }

        if (!PlayerManager.instance.HaveEnoughMoney(skillCost))
            return;

        unlock = true;
        skillImage.color = Color.white;
    }

    private void LockSkillSlot()
    {
        foreach (var postSkill in postSkills)
            if (postSkill.unlock)
            {
                Debug.Log("the post skill" + postSkill.name + " is unlocked");
                return;
            }

        PlayerManager.instance.currency += skillCost;
        unlock = false;
        skillImage.color = lockedSkillColor;
    }
}