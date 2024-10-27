using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_CraftList : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Transform craftSlotsParent;
    [SerializeField] private GameObject craftSlotPrefab;
    [SerializeField] private List<ItemData_Equipment> craftEquipments;

    private void Start()
    {
        UI_CraftList uiCraftList = transform.parent.GetChild(0).GetComponentInChildren<UI_CraftList>();
        uiCraftList.UpdateCraftSlots();
        if (uiCraftList.craftEquipments.Count > 0)
            SetupDefaultCraftWindow(uiCraftList.craftEquipments[0]);
        else
            SetupDefaultCraftWindow();
    }

    private void UpdateCraftSlots()
    {
        foreach (UI_CraftSlot craftSlot in craftSlotsParent.GetComponentsInChildren<UI_CraftSlot>())
            Destroy(craftSlot.gameObject);
        foreach (ItemData_Equipment craftEquipment in craftEquipments)
        {
            GameObject newCraftSlot = Instantiate(craftSlotPrefab, craftSlotsParent);
            newCraftSlot.GetComponent<UI_CraftSlot>().SetupCraftSlot(craftEquipment);
        }
    }

    private void SetupDefaultCraftWindow() =>
        UI.instance.ui_CraftWindow.gameObject.SetActive(false);

    private void SetupDefaultCraftWindow(ItemData_Equipment craftEquipment) =>
        UI.instance.ui_CraftWindow.SetupCraftWindow(craftEquipment);

    public void OnPointerDown(PointerEventData eventData) =>
        UpdateCraftSlots();
}