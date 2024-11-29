using UnityEngine;

public class JumpAttackState_Boss : EnemyState
{
    private Enemy_Boss enemy;

    private Vector3 lastPlayerPos;
    private float jumpAttackMovementSpeed;
    public JumpAttackState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss;
    }

    public override void Enter()
    {
        base.Enter();
        lastPlayerPos = enemy.player.position;
        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;

        enemy.bossVisuals.PlaceLandingZone(lastPlayerPos);
        enemy.bossVisuals.EnableWeaponTrails(true);

        float distanceToPlayer = Vector3.Distance(lastPlayerPos, enemy.transform.position);
        jumpAttackMovementSpeed = distanceToPlayer / enemy.travelTimeTarget;

        enemy.FaceTarget(lastPlayerPos, 1000);

        if(enemy.bossWeaponType == BossWeaponType.Hammer)
        {
            enemy.agent.isStopped = false;
            enemy.agent.speed = enemy.runSpeed;
            enemy.agent.SetDestination(lastPlayerPos);
        }
    }


    public override void Update()
    {
        base.Update();

        Vector3 myPos = enemy.transform.position;
        enemy.agent.enabled = !enemy.ManuelMovementActive();

        if (enemy.ManuelMovementActive())
        {
            enemy.transform.position =
                  Vector3.MoveTowards(myPos, lastPlayerPos, jumpAttackMovementSpeed * Time.deltaTime);
        }


        if (triggerCalled)
            stateMachine.ChangeState(enemy.moveState);

    }
    public override void Exit()
    {
        base.Exit();
        enemy.SetJumpAttackOnCooldown();
        enemy.bossVisuals.EnableWeaponTrails(false);

    }
}
