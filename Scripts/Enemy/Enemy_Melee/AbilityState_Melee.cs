using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private Vector3 movementDirection;

    private const float MAX_DISTANCE_FLOAT = 20;
    private float moveSpeed;
    
    public AbilityState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.visuals.EnableWeaponModel(true);

        moveSpeed = enemy.walkSpeed;
        movementDirection = enemy.transform.position + (enemy.transform.forward * MAX_DISTANCE_FLOAT);

    }

    public override void Exit()
    {
        base.Exit();
        enemy.walkSpeed = moveSpeed;
        enemy.anim.SetFloat("RecoveryIndex", 0);
    }

    public override void Update()
    {
        base.Update();

        if(triggerCalled)
            stateMachine.ChangeState(enemy.recoveryState);


        if (enemy.ManuelMovementActive())
        {
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, movementDirection, enemy.walkSpeed * Time.deltaTime);

        }

        if (enemy.ManuelRotationActive())
        {
             enemy.FaceTarget(enemy.player.position);
            movementDirection = enemy.transform.position + (enemy.transform.forward * MAX_DISTANCE_FLOAT);


        }
    }
    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        enemy.ThrowAxe();
        

    }

}
    

