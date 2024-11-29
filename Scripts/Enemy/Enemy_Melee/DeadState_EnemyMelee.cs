using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState_EnemyMelee : EnemyState
{
    private Enemy_Melee enemy;
    public DeadState_EnemyMelee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
        
    }

    public override void Enter()
    {
        base.Enter();
        
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
