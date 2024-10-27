using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ItemTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;

    public void ShowItemTooltip(ItemData_Equipment equipData)
    {
        itemNameText.text = equipData.itemName;
        itemTypeText.text = equipData.equipmentType.ToString();
        itemDescriptionText.text = equipData.GetItemDescription();

        // 如果名字过长就缩小字体
        itemNameText.fontSize = itemNameText.text.Length > 12 ? 32 : 40;

        gameObject.SetActive(true);
    }

    public void HideItemTooltip() => gameObject.SetActive(false);
}