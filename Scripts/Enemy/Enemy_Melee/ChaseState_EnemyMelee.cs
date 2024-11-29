using UnityEngine;

public class ChaseState_EnemyMelee : EnemyState
{
    private Enemy_Melee enemy;

    private float lastTimeUpdatedDestination;

    public ChaseState_EnemyMelee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {

        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.speed = enemy.runSpeed;
        enemy.agent.isStopped = false;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.PlayerInAttackRange())
            stateMachine.ChangeState(enemy.attackState);

        enemy.FaceTarget(GetNextPathPoint());


        if (canUpdateDestination())
        {
            enemy.agent.destination = enemy.player.transform.position;  // enemy konumunu player konumu olarak degistiiryoruz.
        }


    }

    private bool canUpdateDestination()
    {
        if (Time.time > lastTimeUpdatedDestination + .25f)   // Updatede calistirmak pahali oldugu icin cooldown sistemi yapıyoruz.
        {
            lastTimeUpdatedDestination = Time.time;
            return true;
        }
        return false;
    }

}
