using UnityEngine;

public class BattleState_Range : EnemyState
{
    private Enemy_Range enemy;

    public float lastTimeShot = -10;
    private int bulletsShot = 0;

    private int bulletsPerAttack;
    private float weaponCooldown;

    private float coverCheckTimer;
    private bool firstTimeAttack = true;
    public BattleState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();
        SetupValuesForFirstAttack();

        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;

        enemy.visuals.EnableIK(true, true);
        stateTimer = enemy.attackDelay;
    }



   

    public override void Update()
    {
        base.Update();

        if (enemy.IsSeeingPlayer())
            enemy.FaceTarget(enemy.aimmm.position);

        if(enemy.CanThrowGrenade())
            enemy.stateMachine.ChangeState(enemy.throwGrenadeState);


        if (MustAdvancePlayer())
            stateMachine.ChangeState(enemy.advancePlayerState);

        

        ChangeCoverIfShould();

        if (stateTimer > 0)
            return;



        if (WeaponOutOfBullets())
        {

            if (enemy.isUnstoppable() && UnstoppableWalkReady())
            {
                enemy.advanceDuration = weaponCooldown;
                stateMachine.ChangeState(enemy.advancePlayerState);
            }

            if (WeaponOnCooldown())
                AttempToResetWeapon();

            return;

        }

        if (canShoot())
            Shoot();


    }

    private bool MustAdvancePlayer()
    {
        if(enemy.isUnstoppable())
            return false;

        return enemy.IsPlayerOnAggresionRange() == false && ReadyToLeaveCover();

       
    }


    #region weapon

    private void AttempToResetWeapon()
    {
        bulletsShot = 0;
        bulletsPerAttack = enemy.weaponData.GetBulletsPerAttack();
        weaponCooldown = enemy.weaponData.GetWeaponCooldown();


    }

    private bool canShoot() => Time.time > lastTimeShot + 1 / enemy.weaponData.fireRate;
    private bool WeaponOnCooldown() => Time.time > lastTimeShot + weaponCooldown;
    private bool WeaponOutOfBullets() => bulletsShot >= bulletsPerAttack;
    private void Shoot()
    {
        enemy.FireSingleBullet();
        lastTimeShot = Time.time;
        bulletsShot++;
    }

    private void SetupValuesForFirstAttack()
    {
        if (firstTimeAttack)
        {
            //vurup tekrar ilerlemesi icin
            enemy.aggresionRange = enemy.advanceStoppingDistance + 2;

            firstTimeAttack = false;
            bulletsPerAttack = enemy.weaponData.GetBulletsPerAttack();
            weaponCooldown = enemy.weaponData.GetWeaponCooldown();

        }
    }
    #endregion

    #region Cover System

    public bool ReadyToLeaveCover()
    {
        return Time.time > enemy.minCoverTime + enemy.runToCoverState.lastTimeTookCover;
    }

    private void ChangeCoverIfShould()
    {
        if (enemy.coverPerk != CoverPerk.CanTakeAndChangeCover)
            return;


        coverCheckTimer -= Time.deltaTime;

        if (coverCheckTimer < 0)
        {
            coverCheckTimer = .3f;            //check yapılma hızı

            if (ReadyToChangeCover()&&ReadyToLeaveCover())
            {
                if (enemy.CanGetCover())
                    stateMachine.ChangeState(enemy.runToCoverState);

            }
        }

    }

    private bool IsPlayerInClearSight()
    {
        Vector3 directionToPlayer = (enemy.player.transform.position - enemy.transform.position);



        if (Physics.Raycast(enemy.transform.position, directionToPlayer, out RaycastHit hit))
        {
            if (hit.transform.root == enemy.player.root) 
                return true;
        }

        return false;
    }

    private bool isPlayerClose()
    {
        return Vector3.Distance(enemy.transform.position, enemy.player.transform.position) < enemy.safeDistance;
    }

    private bool ReadyToChangeCover()
    {
        bool inDanger = IsPlayerInClearSight() || isPlayerClose();
        bool advanceTimeIsOver = Time.time > enemy.advancePlayerState.lastTimeAdvanced + enemy.advanceDuration;

        return inDanger && advanceTimeIsOver;
    }

    private bool UnstoppableWalkReady()
    {
        float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.transform.position);
        bool outOfStoppingDistance = distanceToPlayer > enemy.advanceStoppingDistance;
        bool unstoppableWalkOnCooldown = Time.time < enemy.weaponData.minWeaponCooldown + enemy.advancePlayerState.lastTimeAdvanced;

        return outOfStoppingDistance && unstoppableWalkOnCooldown == false;
    }
    #endregion
}
