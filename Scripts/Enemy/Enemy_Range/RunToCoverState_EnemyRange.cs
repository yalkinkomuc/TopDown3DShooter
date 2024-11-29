using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunToCoverState_EnemyRange : EnemyState

{
    private Enemy_Range enemy;
    private Vector3 destination;

    public float lastTimeTookCover {  get; private set; }

    public RunToCoverState_EnemyRange(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();
        destination = enemy.currentCover.transform.position;

        enemy.visuals.EnableIK(true,false);
        enemy.agent.speed = enemy.runSpeed;
        enemy.agent.isStopped = false;

        enemy.agent.SetDestination(destination);
    }

    public override void Exit()
    {
        base.Exit();
        lastTimeTookCover = Time.time;
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(GetNextPathPoint());  // duvarın içinden geçmeye çalışmaması için pathpoint 

        if(Vector3.Distance(enemy.transform.position, destination) <.8f ) 
            stateMachine.ChangeState(enemy.battleState);
        
    }
}
