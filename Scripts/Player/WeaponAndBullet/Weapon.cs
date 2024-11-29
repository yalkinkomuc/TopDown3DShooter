
using UnityEngine;

public enum WeaponType
{
    Pistol,
    Revolver,
    Rifle,
    Shotgun,
    Sniper
}

public enum ShootType
{
    Single,
    Auto
}

[System.Serializable] // inspectorda görünür yapmak için.

public class Weapon
{
    public WeaponType weaponType;
    public ShootType shootType;

    public int bulletDamage;

    [Header("Magazine Details")]

    public int bulletsInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;



    public float reloadSpeed { get; private set; }

    public float equipmentSpeed { get; private set; }

    public float gunDistance { get; private set; }

    public float cameraDistance { get; private set; }
    [Space]
    private float defaultFireRate;
    private float fireRate = 1;   //saniye başına mermi
    public int bulletsPerShot { get; private set; }
    private float lastShootTime;

    [Header("Spread")]

    private float baseSpread;
    private float currentSpread = 2;
    private float maximumSpread = 3;
    private float spreadIncreaseRate = 0.15f;
    private float lastSpreadUpdateTime;
    private float spreadCooldown = 1;



    [Header("Burst Fire")]

    private bool burstModeAvailable;
    public bool burstActive;
    public float burstFireDelay { get; private set; }
    private int burstModeBulletsPerShot;
    private float burstModeFireRate;

    
    public WeaponData WeaponData { get; private set; }

    public Weapon(WeaponData weaponData)
    {
        bulletDamage = weaponData.bulletDamage;
        fireRate = weaponData.fireRate;
        weaponType = weaponData.weaponType;

        bulletsPerShot = weaponData.bulletsPerShot;
        shootType = weaponData.shootType;


        baseSpread = weaponData.baseSpread;
        maximumSpread = weaponData.maximumSpread;
        spreadIncreaseRate = weaponData.spreadIncreaseRate;

        reloadSpeed = weaponData.reloadSpeed;
        equipmentSpeed = weaponData.equipmentSpeed;
        gunDistance = weaponData.gunDistance;
        cameraDistance = weaponData.cameraDistance;

        burstActive = weaponData.burstActive;
        burstModeAvailable = weaponData.burstModeAvailable;
        burstModeBulletsPerShot = weaponData.burstBulletsPerShot;
        burstModeFireRate = weaponData.burstFireRate;
        burstFireDelay = weaponData.burstFireDelay;

        bulletsInMagazine = weaponData.bulletsInMagazine;
        magazineCapacity = weaponData.magazineCapacity;
        totalReserveAmmo = weaponData.totalReserveAmmo;


        defaultFireRate = fireRate;
        this.WeaponData = weaponData;   
    }


    #region BurstRegion

    public bool BurstActivated()
    {
        if (weaponType == WeaponType.Shotgun)
        {
            burstFireDelay = 0;
            return true;
        }

        return burstActive;
    }

    public void ToggleBurst()
    {
        if (burstModeAvailable == false)
            return;

        burstActive = !burstActive;

        if (burstActive)
        {
            bulletsPerShot = burstModeBulletsPerShot;
            fireRate = burstModeFireRate;
        }
        else
        {
            bulletsPerShot = 1;
            fireRate = defaultFireRate;

        }
    }





    #endregion


    #region Spread
    private void UpdateSpread()
    {

        if (Time.time > lastSpreadUpdateTime + spreadCooldown)
            currentSpread = baseSpread;
        else
            IncreaseSpread();

        lastSpreadUpdateTime = Time.time;
    }
    public Vector3 ApplySpread(Vector3 originalDirection)
    {
        UpdateSpread();

        float randomizedValue = Random.Range(-currentSpread, currentSpread);

        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue/2, randomizedValue);

        return spreadRotation * originalDirection;

    }
    private void IncreaseSpread()
    {
        currentSpread = Mathf.Clamp(currentSpread + spreadIncreaseRate, baseSpread, maximumSpread);
    }

    #endregion

    #region Reload
    private bool IsBulletEnough() => bulletsInMagazine > 0;

    public bool canReload()
    {
        if (bulletsInMagazine == magazineCapacity)
            return false;

        if (totalReserveAmmo > 0)
        {
            return true;
        }
        return false;
    }

    public void RefillAmmo()
    {
        totalReserveAmmo += bulletsInMagazine;

        int bulletsToReload = magazineCapacity;

        if (bulletsToReload > totalReserveAmmo)
            bulletsToReload = totalReserveAmmo;

        totalReserveAmmo -= bulletsToReload;
        bulletsInMagazine = bulletsToReload;

        if (totalReserveAmmo < 0)
            totalReserveAmmo = 0;
    }
    #endregion

    public bool canShoot() => IsBulletEnough() && ReadyToFire();


    private bool ReadyToFire()
    {


        if (Time.time > lastShootTime + 1 / fireRate)
        {
            lastShootTime = Time.time;
            return true;
        }
        return false;
    }

}
