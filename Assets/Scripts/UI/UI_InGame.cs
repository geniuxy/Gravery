using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider slider;

    [SerializeField] private Image parryCooldownImage;
    [SerializeField] private Image dashCooldownImage;
    [SerializeField] private Image crystalCooldownImage;
    [SerializeField] private Image swordCooldownImage;
    [SerializeField] private Image blackholeCooldownImage;
    [SerializeField] private Image flaskCooldownImage;
    private float flaskCooldown;
    [Header("Crystal stack info")]
    [SerializeField] private GameObject crystalStack;
    [SerializeField] private TextMeshProUGUI crystalStackText;

    private SkillManager skills;

    [Header("Currency info")]
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private float amountIncreaseRate;
    private float currencyUIAmount;

    void Start()
    {
        if (playerStats != null)
            playerStats.onHealthChanged += UpdateHealthValue;

        parryCooldownImage.fillAmount = 0;
        dashCooldownImage.fillAmount = 0;
        crystalCooldownImage.fillAmount = 0;
        swordCooldownImage.fillAmount = 0;
        blackholeCooldownImage.fillAmount = 0;
        flaskCooldownImage.fillAmount = 0;

        skills = SkillManager.instance;
        Invoke(nameof(UpdateHealthValue),.1f);
        Invoke(nameof(InitCurrencyUIAmount), .1f);
        Invoke(nameof(InitCrystalStackAmount), .1f);
    }

    void Update()
    {
        int playerCurrency = PlayerManager.instance.currency;
        if (currencyUIAmount < playerCurrency)
        {
            currencyUIAmount += amountIncreaseRate * Time.deltaTime;
            currencyUIAmount = Mathf.Clamp(currencyUIAmount, 0, playerCurrency);
        }
        else if (currencyUIAmount > playerCurrency)
        {
            currencyUIAmount -= amountIncreaseRate * Time.deltaTime;
            currencyUIAmount = Mathf.Clamp(currencyUIAmount, 0, float.MaxValue);
        }

        currencyText.text = currencyUIAmount == 0 ? "0" : ((int)currencyUIAmount).ToString("#,#");

        InitCrystalStackAmount();
        crystalStackText.text = skills.crystal.GetCrystalStackAmount().ToString();

        CheckCooldownOfSkill(parryCooldownImage, skills.parry.coolDown, skills.parry.coolDownTimer);
        CheckCooldownOfSkill(dashCooldownImage, skills.dash.coolDown, skills.dash.coolDownTimer);
        CheckCooldownOfSkill(crystalCooldownImage, skills.crystal.coolDown, skills.crystal.coolDownTimer);
        CheckCooldownOfSkill(swordCooldownImage, skills.sword.coolDown, skills.sword.coolDownTimer);
        CheckCooldownOfSkill(blackholeCooldownImage, skills.blackhole.coolDown, skills.blackhole.coolDownTimer);

        if (Input.GetKeyDown(KeyCode.Alpha1) && Inventory.instance.GetDevice(EquipmentType.Flask) != null)
            SetCooldownOf(flaskCooldownImage);
        flaskCooldown = Inventory.instance.flaskCoolDown;
        CheckCooldownOfFlask(flaskCooldownImage, flaskCooldown);
    }

    private void InitCurrencyUIAmount() => currencyUIAmount = PlayerManager.instance.currency;

    private void InitCrystalStackAmount()
    {
        if (skills.crystal.multiCrystalUnlock)
            crystalStack.SetActive(true);
        else
            crystalStack.SetActive(false);
    }

    private void UpdateHealthValue()
    {
        slider.maxValue = playerStats.GetMaxHpValue();
        slider.value = playerStats.currentHp;
    }

    private void SetCooldownOf(Image _image)
    {
        if (_image.fillAmount <= 0)
            _image.fillAmount = 1;
    }

    private void CheckCooldownOfFlask(Image _image, float _cooldown)
    {
        if (_image.fillAmount > 0)
            _image.fillAmount -= 1 / _cooldown * Time.deltaTime;
    }

    private void CheckCooldownOfSkill(Image _image, float _cooldown, float _cooldownTimer) =>
        _image.fillAmount = _cooldownTimer > 0 ? _cooldownTimer / _cooldown : 0;
}