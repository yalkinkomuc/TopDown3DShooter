using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="New Weapon Data",menuName ="Weapon System / Weapon Data")]

public class WeaponData : ScriptableObject
{
    public string WeaponName;
    public WeaponType weaponType;
    public ShootType shootType;
    public int bulletsPerShot = 1;
    public float fireRate;


    [Header("Bullet Info")]
    public int bulletDamage;

    [Header("Magazine Details")]

    public int bulletsInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;



    [Header("Burst")]
    public bool burstModeAvailable;
    public bool burstActive;
    public int burstBulletsPerShot;
    public float burstFireRate;
    public float burstFireDelay = .1f;



    [Header("Spread")]
    public float baseSpread;
    public float maximumSpread = 3;
    public float spreadIncreaseRate = 0.15f;

    [Header("Spesifics")]
    [Range(1, 3)]
    public float reloadSpeed=1;
    [Range(1, 3)]
    public float equipmentSpeed =1;
    [Range(4, 100)]
    public float gunDistance = 4;
    [Range(4, 8)]
    public float cameraDistance = 6;

    [Header("UI elements")]
    public Sprite weaponIcon;
}
