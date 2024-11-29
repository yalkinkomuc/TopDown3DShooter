using UnityEngine;

public class AbilityState_Boss : EnemyState
{
    private Enemy_Boss enemy;
    
    public AbilityState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss;
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = enemy.flameThrowDuration;
        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;
        enemy.bossVisuals.EnableWeaponTrails(true);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.SetAbilityCooldown();
        enemy.bossVisuals.ResetBatteries();
        enemy.bossVisuals.EnableWeaponTrails(false);
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(enemy.player.position);

        if (ShouldDisableFlameThrower())
            DisableFlameThrower();

        if (triggerCalled)
            stateMachine.ChangeState(enemy.moveState);

    }

    private bool ShouldDisableFlameThrower() => stateTimer < 0;
   

    public void DisableFlameThrower()
    {
        if (enemy.bossWeaponType != BossWeaponType.FlameThrower)
            return;

        if (enemy.flameThrowActive == false)
            return;

        enemy.ActivateFlameThrower(false);
    }


    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        if (enemy.bossWeaponType == BossWeaponType.FlameThrower)
        {
            enemy.ActivateFlameThrower(true);
            
            enemy.bossVisuals.DischargeBatteries();
            enemy.bossVisuals.EnableWeaponTrails(false);

        }

        if (enemy.bossWeaponType == BossWeaponType.Hammer)
        {
            enemy.ActivateHammer();
        }

    }


}
