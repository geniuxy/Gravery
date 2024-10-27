using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_Options : MonoBehaviour
{
    [SerializeField] private Button deleteArchiveButton;

    private UI_OptionTooltip ui_OptionTooltip;

    // 如果点击deleteArchiveButton，就调用DeleteArchive方法
    void Start()
    {
        ui_OptionTooltip = UI.instance.ui_OptionTooltip;

        deleteArchiveButton.onClick.AddListener(DeleteGameArchive);
    }

    private void DeleteGameArchive()
    {
        try
        {
            SaveManager.instance.DeleteArchive();
            ui_OptionTooltip.ShowStatTooltip("Delete Archive Successfully");
            ui_OptionTooltip.Invoke(nameof(UI_OptionTooltip.HideStatTooltip), 2f);
        }
        catch (Exception e)
        {
            Debug.LogError("Delete Archive Failed: " + e.Message);
        }
    }
}