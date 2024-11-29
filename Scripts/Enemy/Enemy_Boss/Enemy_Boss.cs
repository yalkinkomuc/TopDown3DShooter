using System.Collections.Generic;
using UnityEngine;


public enum BossWeaponType { FlameThrower, Hammer }

public class Enemy_Boss : Enemy
{
    public float attackRange;
    public EnemyMelee_SFX bossMeleeSFX {  get; private set; }

    [Header("Boss Details")]
    public BossWeaponType bossWeaponType;
    public float actionCooldown = 10;
    public float strikeRange;

    [Header("JumpAttack")]
    public int jumpAttackDamage;
    public float travelTimeTarget = 1;
    public float jumpAttackCooldown;
    private float lastTimeJumped;
    public float minJumpDistanceRequired;
    public Transform impactPoint;

    [Space]
    public float impactRadius = 2.5f;
    public float impactPower = 5;
    [SerializeField] private float upwardsMultiplier = 10;

    [Space]

    [SerializeField] private LayerMask whatToIgnore;

    [Header("FlameThrowAbility")]
    public int flameDamage;
    public float flameThrowDuration;
    public float flameDamageCooldown;
    public bool flameThrowActive { get; private set; }
    public ParticleSystem flameThrower;

    [Header("HammerAbility")]
    public int hammerActiveDamage;
    public GameObject activationPrefab;
    [SerializeField] private float hammerCheckRadius;

    [Header("Ability")]
    public float minAbilityDistance;
    public float abilityCooldown;
    private float lastTimeAbilityUsed;

    [Header("Attack")]
    [SerializeField] private int bossMeleeAttackDamage;
    [SerializeField] private Transform[] damagePoints;
    [SerializeField] private float damageRadius;
    [SerializeField] private GameObject meleeAttackFX;


    #region States
    public IdleState_Boss idleState { get; private set; }
    public MoveState_Boss moveState { get; private set; }
    public AttackState_Boss attackState { get; private set; }
    public JumpAttackState_Boss jumpAttackState { get; private set; }
    public AbilityState_Boss abilityState { get; private set; }
    public EnemyBoss_Visuals bossVisuals { get; private set; }
    public DeadState_Boss deadState { get; private set; }
    public EnemyBoss_FlameThrowerSFX EnemyBossFlameThrowerSFX { get; private set; }
    public Enemy_Boss_HammerSFX Enemy_Boss_HammerSFX { get; private set; }
    #endregion
    protected override void Awake()
    {
        base.Awake();

        bossVisuals = GetComponent<EnemyBoss_Visuals>();

        idleState = new IdleState_Boss(this, stateMachine, "Idle");
        moveState = new MoveState_Boss(this, stateMachine, "Move");
        attackState = new AttackState_Boss(this, stateMachine, "Attack");
        jumpAttackState = new JumpAttackState_Boss(this, stateMachine, "JumpAttack");
        abilityState = new AbilityState_Boss(this, stateMachine, "Ability");
        deadState = new DeadState_Boss(this, stateMachine, "Idle");
        bossMeleeSFX = GetComponent<EnemyMelee_SFX>();
        EnemyBossFlameThrowerSFX = GetComponent<EnemyBoss_FlameThrowerSFX>();
        Enemy_Boss_HammerSFX = GetComponent<Enemy_Boss_HammerSFX>();
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();

        if (ShouldEnterBattleMode())
            EnterBattleMode();

        MeleeAttackCheck(damagePoints, damageRadius, meleeAttackFX,bossMeleeAttackDamage);

    }



    public override void Die()
    {
        base.Die();

        if (stateMachine.currentState != deadState)
            stateMachine.ChangeState(deadState);
    }

    public bool CanUseAbility()
    {

        bool playerInDistance = Vector3.Distance(transform.position, player.position) < minAbilityDistance;

        if (playerInDistance == false)
            return false;

        if (Time.time > lastTimeAbilityUsed + abilityCooldown)
        {
            return true;
        }
        return false;
    }

    public void SetAbilityCooldown() => lastTimeAbilityUsed = Time.time;

    public override void EnterBattleMode()
    {
        if (inBattleMode)
            return;

        base.EnterBattleMode();
        stateMachine.ChangeState(moveState);
    }

    public bool PlayerInAttackRange() => Vector3.Distance(transform.position, player.position) < attackRange;

    public void ActivateFlameThrower(bool activate)
    {

        flameThrowActive = activate;

        if (!activate)
        {
            flameThrower.Stop();
            AudioManager.instance.SFXWithDelayAndFade(EnemyBossFlameThrowerSFX.flameThrowerSFX, false, 0f, .25f);
            anim.SetTrigger("StopFlameThrower");
            return;
        }

        var mainModule = flameThrower.main;
        var extraModule = flameThrower.transform.GetChild(0).GetComponent<ParticleSystem>().main;

        mainModule.duration = flameThrowDuration;
        extraModule.duration = flameThrowDuration;

        flameThrower.Clear();
        flameThrower.Play();
        AudioManager.instance.SFXWithDelayAndFade(EnemyBossFlameThrowerSFX.flameThrowerSFX, true, .4f, 0);


    }
    public void ActivateHammer()
    {
        GameObject newHammerFxPrefab = ObjectPool.instance.GetObject(activationPrefab, impactPoint);
        ObjectPool.instance.ReturnObject(newHammerFxPrefab, 1);

        MassDamage(damagePoints[0].position, hammerCheckRadius,hammerActiveDamage);

    }
    public bool CanDoJumpAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < minJumpDistanceRequired)
            return false;

        if (Time.time > lastTimeJumped + jumpAttackCooldown && IsPlayerOnClearSight())
        {

            return true;
        }

        return false;
    }

    public void JumpImpact()
    {
        Transform impactPoint = this.impactPoint;

        if (impactPoint == null)
            impactPoint = transform;

        MassDamage(impactPoint.position,impactRadius,jumpAttackDamage);
        //AudioManager.instance.SFXWithDelayAndFade(Enemy_Boss_HammerSFX.JumpAttackSFX, true, 0.6f, 0);
    }

    private void MassDamage(Vector3 impactPoint , float impactRadius,int damage)
    {

        HashSet<GameObject> uniqueEntities = new HashSet<GameObject>();

        Collider[] colliders = Physics.OverlapSphere(impactPoint, impactRadius, ~whatisAlly);

        foreach (Collider hit in colliders)
        {

            IDamageble damageble = hit.GetComponent<IDamageble>();

            if (damageble != null)
            {
                GameObject rootEntity = hit.transform.root.gameObject;

                if (uniqueEntities.Add(rootEntity) == false)
                    continue;

                Debug.Log(hit.transform.root.name + "wasDamaged");


                damageble.TakeDamage(damage);
            }

            ApplyPhysicalForceTo(impactPoint, impactRadius, hit);
        }
    }

    private void ApplyPhysicalForceTo(Vector3 impactPoint, float impactRadius, Collider hit)
    {
        Rigidbody rb = hit.GetComponent<Rigidbody>();

        if (rb != null)
            rb.AddExplosionForce(impactPower, impactPoint, impactRadius, upwardsMultiplier, ForceMode.Impulse);
    }

    public void SetJumpAttackOnCooldown() => lastTimeJumped = Time.time;
    public bool IsPlayerOnClearSight()
    {
        Vector3 bossPos = transform.position + new Vector3(0, 1.5f, 0);  // bossun boyu 1.5f oldugu icin
        Vector3 playerPos = player.position + Vector3.up;
        Vector3 directionToPlayer = (playerPos - bossPos).normalized;

        if (Physics.Raycast(bossPos, directionToPlayer, out RaycastHit hit, 100, ~whatToIgnore))
        {
            if (hit.transform.root == player.root)
                return true;
        }

        return false;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (player != null)
        {
            Vector3 bossPos = transform.position + new Vector3(0, 1.5f, 0);  // bossun boyu 1.5f oldugu icin
            Vector3 playerPos = player.position + Vector3.up;

            Gizmos.color = Color.red;

            Gizmos.DrawLine(bossPos, playerPos);


        }
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, minJumpDistanceRequired);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, impactRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, minAbilityDistance);

        if (damagePoints.Length > 0)
        {
            foreach (var damagePoint in damagePoints)
            {
                Gizmos.DrawWireSphere(damagePoint.position, damageRadius);

            }
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(damagePoints[0].position, hammerCheckRadius);

        }
    }
}
