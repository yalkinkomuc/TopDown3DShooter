using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Enemy Data / Range Weapon Data")]

public class EnemyRange_WeaponData : ScriptableObject
{
    [Header("Weapon Details")]
    public Enemy_Range_WeaponType weaponType;
    public float fireRate =1f; // saniye başına mermi

    public int minBulletPerAttack = 1;
    public int maxBulletPerAttack = 1;

    public float minWeaponCooldown = 2;
    public float maxWeaponCooldown = 3;

    [Header("Bullet Details")]
    public int bulletDamage;
    [Space]
    public float bulletSpeed = 20;
    public float bulletSplash = .1f;

    public int GetBulletsPerAttack() => Random.Range(minBulletPerAttack, maxBulletPerAttack+1); //random range oldugu icin +1
    public float GetWeaponCooldown() => Random.Range(minWeaponCooldown, maxWeaponCooldown);

    public Vector3 ApplyWeaponSpread(Vector3 originalDirection)
    {
        

        float randomizedValue = Random.Range(-bulletSplash,bulletSplash);

        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue/2, randomizedValue);

        return spreadRotation * originalDirection;

    }

}
