using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_SkillTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillDescriptionText;
    [SerializeField] private TextMeshProUGUI skillCostText;

    public void ShowTooltip(string _skillName, string _skillDescription,int _skillCost)
    {
        skillNameText.text = _skillName;
        skillDescriptionText.text = _skillDescription;
        skillCostText.text = "Cost: " + _skillCost;
        gameObject.SetActive(true);
    }

    public void HideTooltip() => gameObject.SetActive(false);
}