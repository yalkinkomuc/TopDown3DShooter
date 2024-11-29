using System;
using System.Collections.Generic;
using UnityEngine;


public enum CoverPerk { Unavailable, CanTakeCover, CanTakeAndChangeCover }
public enum UnstoppablePerk { Unavailable, Unstoppable }

public enum GrenadePerk { Unavailable, CanThrowGrenade }
public class Enemy_Range : Enemy

{
    [Header("EnemyPerks")]
    public Enemy_Range_WeaponType weaponType;
    public CoverPerk coverPerk;
    public UnstoppablePerk unstoppablePerk;
    public GrenadePerk grenadePerk;

    [Header("Advance Perks")]
    public float advanceSpeed;
    public float advanceStoppingDistance;
    public float advanceDuration = 2.5f;

    [Header("Grenade Perks")]
    public int grenadeDamage;
    public GameObject grenadePrefab;
    public float explosionTimer = .75f;
    public float timeToTarget = 1.2f;
    public float grenadeCooldown;
    private float lastTimeGrenadeThrown = -10;
    public float impactPower;
    [SerializeField] private Transform grenadeStartPoint;


    [Header("Weapon Details")]

    public EnemyRange_WeaponData weaponData;
    public float attackDelay;

    [Header("Aim Details")]
    public float slowAim = 4;
    public float fastAim = 20;
    public Transform aimmm;
    public Transform playersBody;
    public LayerMask whatToIgnore;



    [Space]

    public Transform weaponHolder;
    public GameObject bulletPrefab;
    public Transform gunPoint;

    [Space]

    [SerializeField] List<EnemyRange_WeaponData> availableWeaponData;


    [Space]
    [Header("CoverSystem")]
    public float minCoverTime;
    public float safeDistance;
    public CoverPoint currentCover { get; private set; }
    public CoverPoint lastCover { get; private set; }

    #region States
    public IdleState_EnemyRange idleState { get; private set; }
    public MoveState_EnemyRange moveState { get; private set; }
    public BattleState_Range battleState { get; private set; }
    public RunToCoverState_EnemyRange runToCoverState { get; private set; }
    public AdvancePlayerState_Range advancePlayerState { get; private set; }
    public ThrowGrenadeState_EnemyRange throwGrenadeState { get; private set; }
    public DeadState_EnemyRange enemyRangeDeadState { get; private set; }
    #endregion


    protected override void Awake()
    {
        base.Awake();

        idleState = new IdleState_EnemyRange(this, stateMachine, "Idle");
        moveState = new MoveState_EnemyRange(this, stateMachine, "Move");
        battleState = new BattleState_Range(this, stateMachine, "Battle");
        runToCoverState = new RunToCoverState_EnemyRange(this, stateMachine, "RunToCover");
        advancePlayerState = new AdvancePlayerState_Range(this, stateMachine, "Advance");
        throwGrenadeState = new ThrowGrenadeState_EnemyRange(this, stateMachine, "ThrowGrenade");
        playersBody = player.GetComponent<Player>().playerBody;
        enemyRangeDeadState = new DeadState_EnemyRange(this, stateMachine, "Idle");

    }

    protected override void Start()
    {
        base.Start();

        playersBody = player.GetComponent<Player>().playerBody; //degisebilir.
        aimmm.parent = null;

        InitializeSpeciality();

        stateMachine.Initialize(idleState);
        visuals.SetupRandomLook();
        SetupWeaponData();


    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();
    }

    public override void Die()
    {
        base.Die();


        if (stateMachine.currentState != enemyRangeDeadState)
            stateMachine.ChangeState(enemyRangeDeadState);


    }

    protected override void InitializeSpeciality()
    {
        if (weaponType == Enemy_Range_WeaponType.Random)
        {
            ChooseRandomweaponType();

        }

        if (isUnstoppable())
        {
            advanceSpeed = 1;
            anim.SetFloat("AdvanceAnimIndex", 1); // 1 yavaş yürümek için

        }
    }

    private void ChooseRandomweaponType()
    {
        List<Enemy_Range_WeaponType> validTypes = new List<Enemy_Range_WeaponType>();

        foreach (Enemy_Range_WeaponType value in Enum.GetValues(typeof(Enemy_Range_WeaponType)))
        {
            if (value != Enemy_Range_WeaponType.Random && value != Enemy_Range_WeaponType.Rifle)
                validTypes.Add(value);
        }

        int randomIndex = UnityEngine.Random.Range(0, validTypes.Count);
        weaponType = validTypes[randomIndex];
    }

    public bool CanThrowGrenade()
    {
        if (grenadePerk == GrenadePerk.Unavailable)
            return false;


        if (Vector3.Distance(player.transform.position, transform.position) < safeDistance)
            return false;

        if (Time.time > grenadeCooldown + lastTimeGrenadeThrown)
            return true;

        return false;
    }

    public void ThrowGrenade()
    {
        lastTimeGrenadeThrown = Time.time;
        visuals.EnableGrenadeModel(false);

        GameObject newGrenade = ObjectPool.instance.GetObject(grenadePrefab, grenadeStartPoint);
        newGrenade.transform.position = grenadeStartPoint.position; //weapon holder sağ elde olduğuı için kullanıyoruz.

        Enemy_Grenade newGrenadeScript = newGrenade.GetComponent<Enemy_Grenade>();

        if (stateMachine.currentState == enemyRangeDeadState)
        {
            newGrenadeScript.SetupGrenade(whatisAlly, transform.position, 1, explosionTimer, impactPower, grenadeDamage);
            return;

        }

        newGrenadeScript.SetupGrenade(whatisAlly, player.transform.position, timeToTarget, explosionTimer, impactPower, grenadeDamage);


    }



    public override void EnterBattleMode()
    {

        if (inBattleMode)
            return;

        base.EnterBattleMode();

        if (CanGetCover())
            stateMachine.ChangeState(runToCoverState);
        else
            stateMachine.ChangeState(battleState);
    }



    #region Cover System

    public bool CanGetCover()
    {
        if (coverPerk == CoverPerk.Unavailable)
            return false;

        currentCover = AttempToFindCover()?.GetComponent<CoverPoint>();

        if (lastCover != currentCover && currentCover != null)
            return true;

        Debug.LogWarning("No Cover Found");
        return false;
    }


    private Transform AttempToFindCover()
    {
        List<CoverPoint> collectedCoverPoints = new List<CoverPoint>();

        foreach (Cover cover in CollectNearbyCovers())
        {
            collectedCoverPoints.AddRange(cover.GetValidCoverPoints(transform));
        }

        CoverPoint ClosestCoverPoint = null;
        float shortestDistance = float.MaxValue;

        foreach (CoverPoint coverPoint in collectedCoverPoints)
        {
            float currentDistance = Vector3.Distance(transform.position, coverPoint.transform.position);

            if (currentDistance < shortestDistance)
            {
                ClosestCoverPoint = coverPoint;
                shortestDistance = currentDistance;
            }

        }

        if (ClosestCoverPoint != null)
        {
            lastCover?.SetOccupied(false);
            lastCover = currentCover;

            currentCover = ClosestCoverPoint;
            currentCover.SetOccupied(true);

            return currentCover.transform;

        }

        return null;

    }

    private List<Cover> CollectNearbyCovers()
    {

        float coverCheckRadius = 30;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, coverCheckRadius); //30 yarıçapında daire çizip Collider arrayinde depoluyoruz.
        List<Cover> collectedCovers = new List<Cover>();

        foreach (Collider collider in hitColliders)  //collider arrayinde her bir hitcollider için
        {
            Cover cover = collider.GetComponent<Cover>();  //Cover scriptine erişiyoruz

            if (cover != null && collectedCovers.Contains(cover) == false)  //eğer cover varsa ve aynı cover objesinden birden fazla yoksa
                collectedCovers.Add(cover);
        }

        return collectedCovers;

    }




    #endregion


    public void FireSingleBullet()
    {
        anim.SetTrigger("Shoot");

        Vector3 bulletsDirection = (aimmm.position - gunPoint.position).normalized; //oyuncunun pozisyonunun yüksekliğine +1 veriyoruz çünkü pivot point ayakta, silahın ucununn konumundan çıakrıp normalized ile vektör uzunluğunu alıyoruz.

        GameObject newBullet = ObjectPool.instance.GetObject(bulletPrefab, gunPoint);

        newBullet.transform.rotation = Quaternion.LookRotation(gunPoint.forward);

        newBullet.GetComponent<Bullet>().SetupBullet(whatisAlly, weaponData.bulletDamage);

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        Vector3 bulletsDirectionWithSpread = weaponData.ApplyWeaponSpread(bulletsDirection);

        rbNewBullet.mass = 20 / weaponData.bulletSpeed;
        rbNewBullet.velocity = bulletsDirectionWithSpread * weaponData.bulletSpeed;

    }

    private void SetupWeaponData()
    {
        List<EnemyRange_WeaponData> filteredData = new List<EnemyRange_WeaponData>();

        foreach (var weaponData in availableWeaponData)
        {
            if (weaponData.weaponType == weaponType)
                filteredData.Add(weaponData);
        }

        if (filteredData.Count > 0)
        {

            int random = UnityEngine.Random.Range(0, filteredData.Count);
            weaponData = filteredData[random];
        }
        else
            Debug.LogWarning("No available data was found");

        gunPoint = visuals.currentWeaponModel.GetComponent<Enemy_Range_WeaponModel>().gunPoint;


    }


    #region Enemy's Aim Region

    public void UpdateAimPosition()
    {
        float aimspeed = AimOnPlayer() ? fastAim : slowAim;
        aimmm.position = Vector3.MoveTowards(aimmm.position, playersBody.position, aimspeed * Time.deltaTime);
    }
    public bool AimOnPlayer()
    {
        float distanceAimToPlayer = Vector3.Distance(aimmm.position, player.position);

        return distanceAimToPlayer < 2;
    }

    public bool IsSeeingPlayer()
    {
        Vector3 myPosition = transform.position + Vector3.up;  // pivot point aşağıda olduğu için 1 birim yukarı
        Vector3 directionToPlayer = playersBody.position - myPosition;

        if (Physics.Raycast(myPosition, directionToPlayer, out RaycastHit hit, Mathf.Infinity, ~whatToIgnore))
        {
            if (hit.transform.root == player.root)
            {
                UpdateAimPosition();
                return true;

            }







        }

        return false;
    }


    #endregion

    public bool isUnstoppable() => unstoppablePerk == UnstoppablePerk.Unstoppable;

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, advanceStoppingDistance);
    }
}
