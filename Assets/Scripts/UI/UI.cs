using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour, ISaveManager
{
    public static UI instance;

    [SerializeField] private GameObject character;
    [SerializeField] private GameObject skill_Tree;
    [SerializeField] private GameObject craft;
    [SerializeField] private GameObject option;
    [SerializeField] private GameObject inGame;
    [Header("End Screen info")]
    public UI_DarkScreen darkScreen;
    [SerializeField] private GameObject diedText;
    [SerializeField] private GameObject restartButton;
    [Header("UI menu info")]
    public UI_ItemTooltip ui_ItemTooltip;
    public UI_StatTooltip ui_StatTooltip;
    public UI_SkillTooltip ui_SkillTooltip;
    public UI_CraftWindow ui_CraftWindow;
    public UI_OptionTooltip ui_OptionTooltip;
    [Header("Option info")]
    [SerializeField] private UI_VolumeSlider[] volumeSliders;
    [SerializeField] private Toggle[] toggles;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        instance = this;
    }

    private void Start()
    {
        SwitchTo(inGame);
        darkScreen.gameObject.SetActive(true);
        ui_ItemTooltip.gameObject.SetActive(false);
        ui_StatTooltip.gameObject.SetActive(false);
        ui_SkillTooltip.gameObject.SetActive(false);
        ui_OptionTooltip.gameObject.SetActive(false);
    }

    private void Update()
    {
        // 防止屏幕黑了以后仍然可以切换menu
        if (darkScreen.GetComponent<Image>().color.a * 255 > 1)
            return;
        if (Input.GetKeyDown(KeyCode.C))
            SwitchWithKeyTo(character);
        if (Input.GetKeyDown(KeyCode.O))
            SwitchWithKeyTo(option);
        if (Input.GetKeyDown(KeyCode.N))
            SwitchWithKeyTo(skill_Tree);
        if (Input.GetKeyDown(KeyCode.B))
            SwitchWithKeyTo(craft);
        if (Input.GetKeyDown(KeyCode.Escape))
            SwitchWithKeyTo(inGame);
    }

    public void SwitchTo(GameObject _menu)
    {
        foreach (Transform child in transform)
            if (child.GetComponent<UI_DarkScreen>() == null)
                child.gameObject.SetActive(false);

        if (_menu != null)
        {
            _menu?.SetActive(true);
            AudioManager.instance.PlaySFX(5);
            GameManager.instance?.PauseGame(_menu != inGame);
        }
    }

    public void SwitchWithKeyTo(GameObject _menu)
    {
        if (_menu != null && _menu.activeSelf)
        {
            _menu.SetActive(false);
            SwitchTo(inGame);
            return;
        }

        SwitchTo(_menu);
    }

    public void SwitchToEndScreen()
    {
        SwitchTo(null); // UI都取消掉
        darkScreen.FadeOut();
        StartCoroutine(ShowDiedText(2.5f));
    }

    private IEnumerator ShowDiedText(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        diedText.SetActive(true);

        AudioManager.instance.PlaySFX(6);

        yield return new WaitForSeconds(1f);
        restartButton.SetActive(true);
    }

    public void LoadData(GameData _data)
    {
        foreach (var pair in _data.toggles)
        {
            foreach (var toggle in toggles)
            {
                if (_data.toggles.TryGetValue(toggle.gameObject.name, out bool value))
                    toggle.isOn = value;
            }
        }

        foreach (var pair in _data.volumeSettings)
        {
            foreach (var slider in volumeSliders)
            {
                if (slider.volumeType == pair.Key)
                    slider.LoadVolumeValue(pair.Value);
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.volumeSettings.Clear();
        _data.toggles.Clear();

        foreach (var toggle in toggles)
            _data.toggles.Add(toggle.gameObject.name, toggle.isOn);
        foreach (var slider in volumeSliders)
            _data.volumeSettings.Add(slider.volumeType, slider.slider.value);
    }
}