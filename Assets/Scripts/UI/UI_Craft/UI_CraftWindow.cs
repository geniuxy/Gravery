using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftWindow : MonoBehaviour
{
    [SerializeField] private Image craftIcon;
    [SerializeField] private TextMeshProUGUI craftName;
    [SerializeField] private TextMeshProUGUI craftDescription;
    [SerializeField] private Button craftButton;
    [SerializeField] private Image[] materialList;

    public void SetupCraftWindow(ItemData_Equipment _data)
    {
        craftButton.onClick.RemoveAllListeners();

        foreach (Image materialIcon in materialList)
        {
            materialIcon.color = Color.clear;
            materialIcon.GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;
        }

        if (_data.craftMaterials.Count > materialList.Length)
        {
            Debug.LogWarning("the material slots is not enough!");
            return;
        }

        for (int i = 0; i < _data.craftMaterials.Count; i++)
        {

            materialList[i].sprite = _data.craftMaterials[i].itemData.itemIcon;
            materialList[i].color = Color.white;

            TextMeshProUGUI materialAmountText = materialList[i].GetComponentInChildren<TextMeshProUGUI>();

            materialAmountText.text = _data.craftMaterials[i].stackSize.ToString();
            materialAmountText.color = Color.white;
        }

        craftIcon.sprite = _data.itemIcon;
        craftName.text = _data.itemName;
        craftDescription.text = _data.GetItemDescription();
        craftButton.onClick.AddListener(() => Inventory.instance.CanCraftEquipment(_data));
    }
}