using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class UI_StatSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //TODO 如果想要界面的名称自定义，可以之后这里加个StatName
    [SerializeField] private StatType statType;
    [SerializeField] private TextMeshProUGUI statNameText;
    [SerializeField] private TextMeshProUGUI statValueText;

    [TextArea]
    [SerializeField] private string statDescription;

    private UI_StatTooltip ui_StatTooltip;

    private void OnValidate()
    {
        gameObject.name = "Stat - " + statType.ToString();
    }

    void Start()
    {
        UpdateStatSlotUI();
        ui_StatTooltip = UI.instance.ui_StatTooltip;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector2 mousePos = Input.mousePosition;
        float xOffset = mousePos.x > Screen.width * .5 ? -15 : 15;
        float yOffset = mousePos.y > Screen.height * .5 ? -15 : 15;
        ui_StatTooltip.transform.position = new Vector2(mousePos.x + xOffset, mousePos.y + yOffset);
        ui_StatTooltip.ShowStatTooltip(statDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui_StatTooltip.HideStatTooltip();
    }

    public void UpdateStatSlotUI()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        if (statNameText != null)
            statNameText.text = statType.ToString() + ": ";
        if (statValueText != null)
            statValueText.text = playerStats?.StatOfType(statType).GetValue().ToString();

        if (statType == StatType.damage)
            statValueText.text =
                (playerStats?.damage.GetValue() +
                 playerStats?.strength.GetValue()).ToString();

        if (statType == StatType.critPower)
            statValueText.text =
                (playerStats?.critPower.GetValue() +
                 playerStats?.strength.GetValue()).ToString();

        if (statType == StatType.evasion)
            statValueText.text =
                (playerStats?.evasion.GetValue() +
                 playerStats?.agility.GetValue()).ToString();

        if (statType == StatType.critChance)
            statValueText.text =
                (playerStats?.critChance.GetValue() +
                 playerStats?.agility.GetValue()).ToString();

        if (statType == StatType.magicResistance)
            statValueText.text =
                (playerStats?.magicResistance.GetValue() +
                 playerStats?.intelligence.GetValue() * 3).ToString();

        if (statType == StatType.maxHp)
            statValueText.text =
                (playerStats?.maxHp.GetValue() +
                 playerStats?.vitality.GetValue() * 5).ToString();
    }
}