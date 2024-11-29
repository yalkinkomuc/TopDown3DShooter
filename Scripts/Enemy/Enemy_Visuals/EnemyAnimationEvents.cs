using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    private Enemy enemy;
    private Enemy_Boss enemyBoss;
    private Enemy_Melee enemyMelee;
    private Enemy_Boss_HammerSFX enemyBossHammerSFX;
    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
        enemyMelee = GetComponentInParent<Enemy_Melee>();   
        enemyBoss = GetComponentInParent<Enemy_Boss>();
        enemyBossHammerSFX = GetComponentInParent<Enemy_Boss_HammerSFX>();
    }

    public void AnimationTrigger() => enemy.AnimationTrigger();
    public void StartManuelMovement() => enemy.ActivateManuelMovement(true);
    public void StopManuelMovement() => enemy.ActivateManuelMovement(false);
    public void StartManuelRotation() => enemy.ActivateManuelRotation(true);
    public void StopManuelRotation() => enemy.ActivateManuelRotation(false);
    public void AbilityEvent() => enemy.AbilityTrigger();

    public void EnableIK() => enemy.visuals.EnableIK(true, true, 1.5f);

    public void EnableWeaponModel()
    {
        enemy.visuals.EnableWeaponModel(true);
        enemy.visuals.EnableSecondaryWeaponModel(false);
    }
    public void BossJumpImpact()
    {
        

        enemyBoss?.JumpImpact();
        AudioManager.instance.SFXWithDelayAndFade(enemyBossHammerSFX.JumpAttackSFX, true, 0.6f, 0);

    }

    public void BeginMeleeAttackCheck()
    {
        enemy?.EnableMeleeAttackCheck(true);
        enemy?.audioManager.PlaySFX(enemyMelee?.meleeSFX.swooshSFX, true);
        enemyBoss?.EnableMeleeAttackCheck(true);
        enemyBoss?.audioManager.PlaySFX(enemyBoss?.bossMeleeSFX.swooshSFX, true);
    }

    public void EndMeleeAttackCheck()
    {
        enemy?.EnableMeleeAttackCheck(false);
        enemyBoss?.EnableMeleeAttackCheck(false);
    }

}
