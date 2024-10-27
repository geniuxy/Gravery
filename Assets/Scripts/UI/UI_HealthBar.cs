using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    private Entity entity;
    private CharacterStats myStats;
    private RectTransform rt;
    private Slider slider;

    private void Start()
    {
        entity = GetComponentInParent<Entity>();
        myStats = GetComponentInParent<CharacterStats>();
        rt = GetComponent<RectTransform>();
        slider = GetComponentInChildren<Slider>();
    }

    // 用invoke是因为enemy出门自带血条，需要invoke执行，不然报错
    private void OnEnable() => Invoke(nameof(InitHealthBar), .1f);

    private void InitHealthBar()
    {
        entity.onFlipped += FlipUI;
        rt.localRotation = Quaternion.Euler(0, GetHealthBarInitRotationY(), 0);
        // 往委托里加入方法
        myStats.onHealthChanged += UpdateHealthValue;
        // 创建UI_Bar之后也要更新数值
        UpdateHealthValue();
    }

    private int GetHealthBarInitRotationY()
    {
        if (entity.defaultFacingDir == 1)
            return entity.facingDir == 1 ? 0 : -180;
        return entity.facingDir == 1 ? -180 : 0;
    }

    private void OnDisable()
    {
        if (entity == null || myStats == null) return;
        entity.onFlipped -= FlipUI;
        myStats.onHealthChanged -= UpdateHealthValue;
    }

    private void UpdateHealthValue()
    {
        slider.maxValue = myStats.GetMaxHpValue();
        slider.value = myStats.currentHp;
    }

    private void FlipUI() => rt.Rotate(0, 180, 0);
}