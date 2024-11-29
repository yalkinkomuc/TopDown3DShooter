using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_EnemyRange : EnemyState
{

    private Enemy_Range enemy;

    public IdleState_EnemyRange(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.visuals.EnableIK(true,false);
        enemy.anim.SetFloat("IdleAnimIndex", Random.Range(0, 3));

        stateTimer = enemy.idleTime;



    }

    public override void Update()
    {
        base.Update();

        if(stateTimer < 0)
           stateMachine.ChangeState(enemy.moveState);

    }
}
