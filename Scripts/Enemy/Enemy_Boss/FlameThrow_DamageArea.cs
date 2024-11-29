using UnityEngine;

public class FlameThrow_DamageArea : MonoBehaviour
{
    private Enemy_Boss enemyBoss;

    private float damageCooldown;
    private float lastTimeDamaged;
    private int flameDamage;
    private void Awake()
    {
        enemyBoss = GetComponentInParent<Enemy_Boss>();
        damageCooldown = enemyBoss.flameDamageCooldown;
        flameDamage = enemyBoss.flameDamage;
    }

    private void OnTriggerStay(Collider other)
    {
        if (enemyBoss.flameThrowActive == false)
            return;

        if (Time.time - lastTimeDamaged < damageCooldown)
            return;




        IDamageble damagable = other.GetComponent<IDamageble>();

        if (damagable != null)
        {
            damagable.TakeDamage(flameDamage);
            lastTimeDamaged = Time.time;
        }

    }
}
