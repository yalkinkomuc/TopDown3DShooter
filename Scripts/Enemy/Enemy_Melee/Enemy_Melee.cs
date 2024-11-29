using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AttackData_EnemyMelee
{
    public int attackDamage;
    public string attackName;
    public float attackRange;
    public float moveSpeed;
    public float attackIndex;
    public float animationSpeed;
    public AttackType_Melee attackType;

}

public enum AttackType_Melee { Close, Charge }
public enum EnemyMelee_Type { Regular, Shield, Dodge, AxeThrow }
public class Enemy_Melee : Enemy
{
    public EnemyMelee_SFX meleeSFX {  get; private set; }

    #region States
    public IdleState_Melee idleState { get; private set; }
    public MoveState_Melee moveState { get; private set; }
    public RecoveryState_Melee recoveryState { get; private set; }
    public ChaseState_EnemyMelee chaseState { get; private set; }
    public AttackState_Melee attackState { get; private set; }
    public DeadState_EnemyMelee deadState { get; private set; }
    public AbilityState_Melee abilityState { get; private set; }

    #endregion

    [Header("Enemy Settings")]
    public EnemyMelee_Type meleeType;
    public Enemy_Melee_WeaponType weaponModelType;

    [Header("Shield")]
    [SerializeField] Transform shieldTransform;
    public int shieldDurability;
    [Space]

    [Header("Dodge")]
    private float lastTimeDodged = -10;
    public float dodgeCooldown;



    [Header("AttackData")]
    
    public AttackData_EnemyMelee attackData;
    public List<AttackData_EnemyMelee> attackList;
    private Enemy_Weapon_Model currentWeapon;
    [SerializeField] private GameObject meleeAttackFX;

    [Header("Axe Throw Ability")]
    public int axeDamage;
    public GameObject axePrefab;
    public float axeFlySpeed;
    public float axeAimTimer;
    public float axeThrowCooldown;
    private float lastTimeAxeThrown;
    public Transform axeStartPoint;

    private bool isAttackReady;

    protected override void Awake()
    {
        base.Awake();

        

        idleState = new IdleState_Melee(this, stateMachine, "Idle");
        moveState = new MoveState_Melee(this, stateMachine, "Move");
        recoveryState = new RecoveryState_Melee(this, stateMachine, "Recovery");
        chaseState = new ChaseState_EnemyMelee(this, stateMachine, "Chase");
        attackState = new AttackState_Melee(this, stateMachine, "Attack");
        deadState = new DeadState_EnemyMelee(this, stateMachine, "Idle");
        abilityState = new AbilityState_Melee(this, stateMachine, "ThrowAxe");

        meleeSFX = GetComponent<EnemyMelee_SFX>();
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
        InitializeSpeciality();
        visuals.SetupRandomLook();
        UpdateAttackData();

    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();

        
            MeleeAttackCheck(currentWeapon.damagePoints,currentWeapon.attackRadius,meleeAttackFX,attackData.attackDamage);

    }

    

    

    public override void EnterBattleMode()
    {
        if (inBattleMode)
            return;


        base.EnterBattleMode();
        stateMachine.ChangeState(recoveryState);
    }

    protected override void InitializeSpeciality()
    {
        if (meleeType == EnemyMelee_Type.AxeThrow)
        {
            weaponModelType = (Enemy_Melee_WeaponType.Throwable);
        }


        if (meleeType == EnemyMelee_Type.Shield)
        {
            anim.SetFloat("ChaseIndex", 1);
            shieldTransform.gameObject.SetActive(true);
            weaponModelType = Enemy_Melee_WeaponType.OneHand;
        }

        if (meleeType == EnemyMelee_Type.Dodge)
        {
            weaponModelType = Enemy_Melee_WeaponType.Unarmed;
        }

    }



    public override void AbilityTrigger()
    {
        base.AbilityTrigger();
        walkSpeed = walkSpeed * .6f;
        visuals.EnableWeaponModel(false);

    }

    public void ThrowAxe()
    {
        GameObject newAxe = ObjectPool.instance.GetObject(axePrefab, axeStartPoint);

        newAxe.GetComponent<Enemy_Axe>().SetupAxe(axeFlySpeed, player, axeAimTimer,axeDamage);

    }


    

    public override void Die()
    {
        base.Die();

        if (stateMachine.currentState != deadState)
            stateMachine.ChangeState(deadState);
                
    }

    public bool PlayerInAttackRange() => Vector3.Distance(transform.position, player.position) < attackData.attackRange;

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackData.attackRange);
    }

    public void ActivateDodgeRoll()
    {

        if (meleeType != EnemyMelee_Type.Dodge)
            return;


        if (stateMachine.currentState != chaseState)
            return;

        if (Vector3.Distance(transform.position, player.position) < 2)
            return;

        float animLength = GetAnimationClipDuration("DodgeRoll_NRO");

        //Debug.Log(animLength);

        if (Time.time > dodgeCooldown + animLength + lastTimeDodged)
        {
            lastTimeDodged = Time.time;
            anim.SetTrigger("Dodge");
        }

    }
    public bool CanThrowAxe()
    {
        if (meleeType != EnemyMelee_Type.AxeThrow)
            return false;

        if (Time.time > lastTimeAxeThrown + axeThrowCooldown)
        {

            lastTimeAxeThrown = Time.time;
            return true;
        }
        return false;
    }

    public float GetAnimationClipDuration(string clipName)
    {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            if (clip.name == clipName)
                return clip.length;
        }

        Debug.Log(clipName + "Not Found In Animations");
        return 0;

    }

    public void UpdateAttackData()
    {
        currentWeapon = visuals.currentWeaponModel.GetComponent<Enemy_Weapon_Model>();

        if (currentWeapon.weaponData != null)
        {
            attackList = new List<AttackData_EnemyMelee>(currentWeapon.weaponData.attackData);
            turnSpeed = currentWeapon.weaponData.turnSpeed;
        }



    }


}
