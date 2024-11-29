using UnityEngine;

public class AttackState_Boss : EnemyState
{
    private Enemy_Boss enemy;
    public float lastTimeAttacked { get; private set; }
    public AttackState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.anim.SetFloat("AttackAnimIndex", Random.Range(0, 2));
        enemy.agent.isStopped = true;
        enemy.bossVisuals.EnableWeaponTrails(true);
        stateTimer = 1f;
    }

    public override void Exit()
    {
        base.Exit();
        lastTimeAttacked = Time.time;
        enemy.bossVisuals.EnableWeaponTrails(false);
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0f)
            enemy.FaceTarget(enemy.player.position, 20);



        if (triggerCalled)
        {
            if (enemy.PlayerInAttackRange())
                stateMachine.ChangeState(enemy.idleState);
            else
                enemy.stateMachine.ChangeState(enemy.moveState);
        }
    }
}
