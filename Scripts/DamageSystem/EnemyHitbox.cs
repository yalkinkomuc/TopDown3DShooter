using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : Hitbox
{
    private Enemy enemy;


    protected override void Awake()
    {
        base.Awake();

        enemy = GetComponentInParent<Enemy>();
    }
    
    public override void TakeDamage(int damage)
    {

        int newDamage = Mathf.RoundToInt(damage * damageMultiplier);

        enemy.GetHit(newDamage);
    }
}
