using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : HealthController
{
    private Player player;
    public bool isDead {  get; private set; }
    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<Player>();
    }

    public override void ReduceHealth(int damage)
    {
        base.ReduceHealth(damage);

        if (ShouldDie())
            Die();

        UI.instance.inGameUI.UpdateHealthUI(currentHealth,maxHealth);
            
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;
        player.animator.enabled = false;
        player.ragdoll.RagdollActive(true);

        GameManager.instance.GameOver();
    }
}
