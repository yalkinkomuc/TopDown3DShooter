using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState_EnemyRange : EnemyState
{
    private Enemy_Range enemy;

    public DeadState_EnemyRange(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();

        if (enemy.throwGrenadeState.finishedThrowingGrenade == false)
            enemy.ThrowGrenade();

        
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }
}
