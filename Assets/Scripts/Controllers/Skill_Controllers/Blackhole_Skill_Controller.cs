using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Blackhole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotkeyPrefab;
    [SerializeField] private List<KeyCode> hotkeyList;

    private Player player;
    private SkillManager skillManager;

    private float maxSize;
    private float growSpeed;
    private float shrinkSpeed;
    private float blackholeTimer;

    private bool canGrow = true;
    private bool canShrink;
    private bool canCloneAttack;
    private bool canCreateHotkey = true;
    private bool isShootingCrystal;

    private int amountOfCloneAttack;
    private float cloneAttackCoolDown;
    private float cloneAttackTimer;

    private List<Transform> targets = new List<Transform>();
    private List<GameObject> createdHotkey = new List<GameObject>();

    public bool playerCanExitState { get; private set; }

    public void SetupBlackHole(float _maxSize, float _growSpeed, float _shrinkSpeed, int _amountOfCloneAttack,
        float _cloneAttackCoolDown, float _blackholeDuration)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountOfCloneAttack = _amountOfCloneAttack;
        cloneAttackCoolDown = _cloneAttackCoolDown;
        blackholeTimer = _blackholeDuration;
        isShootingCrystal = skillManager.clone.crystalInsteadOfClone;
    }

    private void Awake()
    {
        player = PlayerManager.instance.player;
        skillManager = SkillManager.instance;
    }

    public void Update()
    {
        cloneAttackTimer -= Time.deltaTime;
        blackholeTimer -= Time.deltaTime;

        // 黑洞持续时间到了释放技能并退出
        if (blackholeTimer < 0)
        {
            // 设置为无限大，确保这个逻辑只执行一次
            blackholeTimer = Mathf.Infinity;
            ExitBlackhole();
        }

        // 按下R即释放技能
        if (Input.GetKeyDown(KeyCode.R) && !canShrink)
            ReleaseAttack();

        AttackLogic();

        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize),
                growSpeed * Time.deltaTime);
        }

        if (canShrink)
        {
            transform.localScale =
                Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);

            if (transform.localScale.x < 0)
                Destroy(gameObject);
        }
    }

    # region Blackhole Clone Attack

    private void ReleaseAttack()
    {
        DestroyHotkey();
        canCloneAttack = true;
        canCreateHotkey = false;
        if (!isShootingCrystal)
            player.fx.MakeTransparent(true);
    }

    private void AttackLogic()
    {
        if (cloneAttackTimer < 0 && canCloneAttack)
        {
            cloneAttackTimer = cloneAttackCoolDown;

            if (targets.Count > 0)
                Attack();
            else
                ExitBlackhole();
        }
    }

    private void Attack()
    {
        int randomIndex = Random.Range(0, targets.Count);

        float xOffset = Random.Range(0, 100) > 50 ? 1 : -1;

        if (amountOfCloneAttack > 0)
        {
            if (isShootingCrystal)
            {
                skillManager.crystal.CreateCrystal();
                skillManager.crystal.CrystalToRandomEnemy();
            }
            else
                skillManager.clone.CreateClone(targets[randomIndex], 0, new Vector3(xOffset, 0));

            amountOfCloneAttack--;
        }
        else
            Invoke(nameof(ExitBlackhole), .5f);
    }

    private void ExitBlackhole()
    {
        DestroyHotkey();
        canShrink = true;
        canCloneAttack = false;
        playerCanExitState = true;
        player.fx.MakeTransparent(false);
    }

    # endregion

    # region Trigger

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(true);

            CreateHotkey(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) => collision.GetComponent<Enemy>()?.FreezeTime(false);

    # endregion

    # region Hotkey

    private void CreateHotkey(Collider2D collision)
    {
        if (hotkeyList.Count <= 0)
        {
            Debug.LogWarning("No enough hotkeys in the key code list");
            return;
        }

        if (!canCreateHotkey)
            return;

        GameObject newHotkey = Instantiate(hotkeyPrefab, collision.transform.position + new Vector3(0, 2),
            Quaternion.identity);
        createdHotkey.Add(newHotkey);

        KeyCode chosenHotkey = hotkeyList[Random.Range(0, hotkeyList.Count)];
        hotkeyList.Remove(chosenHotkey);

        Blackhole_Hotkey_Controller hotkeyController = newHotkey.GetComponentInChildren<Blackhole_Hotkey_Controller>();

        hotkeyController.SetupHotkey(chosenHotkey, collision.transform, this);
    }

    private void DestroyHotkey()
    {
        if (createdHotkey.Count <= 0)
            return;
        foreach (var hotkey in createdHotkey)
        {
            Destroy(hotkey);
        }
    }

    public void AddEnemyToList(Transform _enemy) => targets.Add(_enemy);

    # endregion
}