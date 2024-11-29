using UnityEngine;

public class MoveState_Boss : EnemyState
{
    private Enemy_Boss enemy;
    private Vector3 destination;

    private float actionTimer;
    private float timeBeforeSpeedUp = 5;
    private bool isSpeedUpActive;
    public MoveState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss;

    }

    public override void Enter()
    {
        base.Enter();
        SpeedReset();
        enemy.agent.isStopped = false;
        destination = enemy.GetPatrolDestination();
        enemy.agent.SetDestination(destination);

        actionTimer = enemy.actionCooldown;
    }

    private void SpeedReset()
    {
        isSpeedUpActive = false;
        enemy.anim.SetFloat("MoveAnimSpeedMultiplier", 1);
        enemy.anim.SetFloat("MoveAnimIndex", 0);
        enemy.agent.speed = enemy.walkSpeed;
    }

    private bool ShouldSpeedUp()
    {
        if (isSpeedUpActive)
            return false;

        if (Time.time > enemy.attackState.lastTimeAttacked + timeBeforeSpeedUp)
        {
            return true;
        }
        return false;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        actionTimer -= Time.deltaTime;
        enemy.FaceTarget(GetNextPathPoint());

        if (enemy.inBattleMode)
        {

            if (ShouldSpeedUp())
                SpeedUp();




            Vector3 playerPos = enemy.player.position;
            enemy.agent.SetDestination(playerPos);

            if (actionTimer < 0)
            {
                PerformRandomAction();
            }


            else if (enemy.PlayerInAttackRange())
                stateMachine.ChangeState(enemy.attackState);
        }
        else
        {
            if (Vector3.Distance(enemy.transform.position, destination) < .25f)
                stateMachine.ChangeState(enemy.idleState);

        }


    }

    private void SpeedUp()
    {
        enemy.agent.speed = enemy.runSpeed;
        enemy.anim.SetFloat("MoveAnimIndex", 1);
        enemy.anim.SetFloat("MoveAnimSpeedMultiplier", 1.5f);
        isSpeedUpActive = true;
    }

    private void PerformRandomAction()
    {
        actionTimer = enemy.actionCooldown;


        if (Random.Range(0, 2) == 0)  // %50 şansla 0 ya da 1 gelicek ve ona göre skill atıcak.
        {
            TryAbility();
        }
        else
        {

            if (enemy.CanDoJumpAttack())
                stateMachine.ChangeState(enemy.jumpAttackState);
            else if (enemy.bossWeaponType == BossWeaponType.Hammer)
                TryAbility();

        }
    }

    private void TryAbility()
    {
        if (enemy.CanUseAbility())
            stateMachine.ChangeState(enemy.abilityState);
    }
}


