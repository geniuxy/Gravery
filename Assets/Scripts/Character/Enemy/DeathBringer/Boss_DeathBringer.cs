using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Boss_DeathBringer : Enemy
{
    public bool isBossFightBegin;
    public Vector3 defaultPosition { get; private set; }

    [Header("Teleport settings")]
    [SerializeField] private BoxCollider2D teleportArea;
    [SerializeField] private Vector2 surroundingCheckSize; // 注意这里surroundingCheckSize 不能超过怪物的碰撞体大小
    [SerializeField] private float defaultChanceToTeleport;
    [HideInInspector] public float chanceToTeleport;

    [Header("Spell cast settings")]
    [SerializeField] private GameObject spellCastPrefab;
    [SerializeField] private float spellStateCoolDown;
    [HideInInspector] public float lastTimeSpellState;
    public float minSpellCastCoolDown = .8f;
    public float maxSpellCastCoolDown = 1.5f;
    public int amountOfSpells = 5;

    # region States

    public DeathBringerIdleState idleState { get; private set; }
    public DeathBringerMoveState moveState { get; private set; }
    public DeathBringerBattleState battleState { get; private set; }
    public DeathBringerAttackState attackState { get; private set; }
    public DeathBringerTeleportState teleportState { get; private set; }
    public DeathBringerSpellState spellState { get; private set; }
    public DeathBringerDeadState deadState { get; private set; }

    # endregion

    protected override void Awake()
    {
        base.Awake();

        idleState = new DeathBringerIdleState(stateMachine, this, this, "Idle");
        moveState = new DeathBringerMoveState(stateMachine, this, this, "Move");
        battleState = new DeathBringerBattleState(stateMachine, this, this, "Move");
        attackState = new DeathBringerAttackState(stateMachine, this, this, "Attack");
        teleportState = new DeathBringerTeleportState(stateMachine, this, this, "Teleport");
        spellState = new DeathBringerSpellState(stateMachine, this, this, "SpellCast");
        deadState = new DeathBringerDeadState(stateMachine, this, this, "Idle");
    }

    protected override void Start()
    {
        base.Start();

        // 解决初始Animator sprite朝向左的问题
        SetupDefaultFacingDir(-1);
        // 怪的初始朝向希望与默认朝向不一致时，翻转
        if (defaultFacingDir != initFacingDir)
            Flip();

        defaultPosition = transform.position;
        stateMachine.Initialize(idleState);
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }

    public bool CanEnterSpellState() => Time.time > spellStateCoolDown + lastTimeSpellState;

    public void CreateSpellCast()
    {
        Player player = PlayerManager.instance.player;

        float xOffset = 0;

        if (player.rb.velocity.x != 0)
            xOffset = player.facingDir * 3;

        Vector3 spellCastPos = new Vector3(player.transform.position.x + xOffset, player.transform.position.y + 1.5f);

        GameObject newSpellCast = Instantiate(spellCastPrefab, spellCastPos, Quaternion.identity);
        newSpellCast.GetComponent<DeathBringerSpellCast_Controller>().SetSpellCast(stats);
    }

    public bool CanTeleport()
    {
        if (Random.Range(0f, 100f) < chanceToTeleport)
        {
            chanceToTeleport = defaultChanceToTeleport;
            return true;
        }

        return false;
    }

    public void FindTeleportPosition()
    {
        float x = Random.Range(teleportArea.bounds.min.x + 3f, teleportArea.bounds.max.x - 3f);
        float y = Random.Range(teleportArea.bounds.min.y + 3f, teleportArea.bounds.max.y - 3f);

        transform.position = new Vector3(x, y);
        transform.position = new Vector3(transform.position.x,
            transform.position.y - GroundBelow().distance + cd.size.y / 2);

        if (SomethingIsAround() || !GroundBelow())
        {
            Debug.Log("Looking for new position");
            FindTeleportPosition();
        }
    }

    public RaycastHit2D GroundBelow() => Physics2D.Raycast(transform.position, Vector2.down, 100, whatIsGround);

    public bool SomethingIsAround() =>
        Physics2D.BoxCast(transform.position, surroundingCheckSize, 0, Vector2.zero, 0, whatIsGround);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawLine(transform.position,
            new Vector3(transform.position.x, transform.position.y - GroundBelow().distance));
        Gizmos.DrawWireCube(transform.position, surroundingCheckSize);
    }
}