using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}

public class Sword_Skill : Skill
{
    public SwordType swordType = SwordType.Regular;

    [Header("Bounce info")]
    [SerializeField] private UI_SkillSlot skillTreeButton_BounceSword;
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceGravity;
    [SerializeField] private float bounceSpeed;

    [Header("Pierce info")]
    [SerializeField] private UI_SkillSlot skillTreeButton_PierceSword;
    [SerializeField] private int pierceAmount;
    [SerializeField] private float pierceGravity;

    [Header("Spin info")]
    [SerializeField] private UI_SkillSlot skillTreeButton_SpinSword;
    [SerializeField] private float maxTravelDistance;
    [SerializeField] private float spinDuration;
    [SerializeField] private float spinGravity;
    [SerializeField] private float hitCoolDown;
    [SerializeField] private float spinSlideSpeed;

    [Header("Skill info")]
    [SerializeField] private UI_SkillSlot skillTreeButton_RegularSword;
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private float regularGravity;
    [SerializeField] private float frozenTimeDuration;
    [SerializeField] private float returnSpeed;
    public bool swordUnlock { get; private set; }
    private float swordGravity;

    [Header("Sword passive")]
    [SerializeField] private UI_SkillSlot skillTreeButton_TimeStop;
    [SerializeField] private UI_SkillSlot skillTreeButton_Vulnerable;
    public bool timeStopUnlock { get; private set; }
    public bool vulnerableUnlock { get; private set; }

    [Header("Aim dots")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBetweenDots;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotParent;

    private GameObject[] dots;
    private Vector2 finalDir;

    protected override void Start()
    {
        base.Start();

        GenerateDots();
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyUp(KeyCode.Mouse1))
            // normalized <=> 模
            finalDir = new Vector2(AimDirection().normalized.x * launchForce.x,
                AimDirection().normalized.y * launchForce.y);

        if (Input.GetKey(KeyCode.Mouse1))
        {
            for (int i = 0; i < dots.Length; i++)
            {
                dots[i].transform.position = DotsPosition(i * spaceBetweenDots);
            }
        }

        SetupGravity();
    }

    // 设置剑的重力
    private void SetupGravity()
    {
        if (swordType == SwordType.Regular)
            swordGravity = regularGravity;
        else if (swordType == SwordType.Bounce)
            swordGravity = bounceGravity;
        else if (swordType == SwordType.Pierce)
            swordGravity = pierceGravity;
        else if (swordType == SwordType.Spin)
            swordGravity = spinGravity;
    }

    // 在扔出剑动作过程中生成一把剑
    public void CreateSword()
    {
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        Sword_Skill_Controller newSwordSkillController = newSword.GetComponent<Sword_Skill_Controller>();

        if (swordType == SwordType.Bounce)
            newSwordSkillController.SetupBounce(true, bounceAmount, bounceSpeed);
        else if (swordType == SwordType.Pierce)
            newSwordSkillController.SetupPierce(pierceAmount);
        else if (swordType == SwordType.Spin)
            newSwordSkillController.SetupSpin(true, maxTravelDistance, spinDuration, hitCoolDown, spinSlideSpeed);

        newSwordSkillController.SetupSword(finalDir, swordGravity, player, frozenTimeDuration, returnSpeed);

        player.AssignNewSword(newSword);

        DotsActive(false);
    }

    # region Unlock skill

    public override void CheckSkill()
    {
        UnlockSword();
        UnlockBounce();
        UnlockPierce();
        UnlockSpin();
        UnlockTimeStop();
        UnlockVulnerable();
    }

    private void UnlockSword()
    {
        if (skillTreeButton_RegularSword.unlock)
        {
            swordUnlock = true;
            swordType = SwordType.Regular;
        }
        else
            swordUnlock = false;
    }

    private void UnlockBounce()
    {
        if (skillTreeButton_BounceSword.unlock)
            swordType = SwordType.Bounce;
    }

    private void UnlockPierce()
    {
        if (skillTreeButton_PierceSword.unlock)
            swordType = SwordType.Pierce;
    }

    private void UnlockSpin()
    {
        if (skillTreeButton_SpinSword.unlock)
            swordType = SwordType.Spin;
    }

    private void UnlockTimeStop() => timeStopUnlock = skillTreeButton_TimeStop.unlock;

    private void UnlockVulnerable() => vulnerableUnlock = skillTreeButton_Vulnerable.unlock;

    # endregion

    #region Aim region

    public Vector2 AimDirection()
    {
        Vector2 playerPos = player.transform.position;
        // 获取鼠标的位置
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 aimDir = mousePos - playerPos;

        return aimDir;
    }

    public void DotsActive(bool _isActive)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].SetActive(_isActive);
        }
    }

    private void GenerateDots()
    {
        dots = new GameObject[numberOfDots];
        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotParent);
            dots[i].SetActive(false);
        }
    }

    private Vector2 DotsPosition(float t)
    {
        // d = vt + 0.5at^2
        Vector2 position = (Vector2)player.transform.position + new Vector2(AimDirection().normalized.x * launchForce.x,
            AimDirection().normalized.y * launchForce.y) * t + .5f * (Physics2D.gravity * swordGravity) * (t * t);

        return position;
    }

    #endregion
}