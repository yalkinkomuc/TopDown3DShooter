using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowGrenadeState_EnemyRange : EnemyState
{
    Enemy_Range enemy;
    public bool finishedThrowingGrenade { get; private set; } = true;
    public ThrowGrenadeState_EnemyRange(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
       enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();
        finishedThrowingGrenade = false;
        enemy.visuals.EnableWeaponModel(false);
        enemy.visuals.EnableIK(false,false);
        enemy.visuals.EnableSecondaryWeaponModel(true);
        enemy.visuals.EnableGrenadeModel(true);
    }

   

    public override void Update()
    {
        base.Update();

        Vector3 playerPos = enemy.player.transform.position + Vector3.up;

        enemy.FaceTarget(playerPos);
        enemy.aimmm.position = playerPos;

        if(triggerCalled)
            stateMachine.ChangeState(enemy.battleState);

    }

    public override void AbilityTrigger()
    {
        finishedThrowingGrenade = true;
        base.AbilityTrigger();
        enemy.ThrowGrenade();
    }

}
