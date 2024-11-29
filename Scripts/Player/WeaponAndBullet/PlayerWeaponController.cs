using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private Player player;
    private const float REFERENCE_BULLET_SPEED = 20;
    // mass formülünden gelen default hız

    [SerializeField] private LayerMask whatIsAlly;

    [SerializeField] private List<WeaponData> defaultWeaponData;
    [SerializeField] public Weapon currentWeapon;

    private bool isWeaponReady;
    private bool isShooting;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletImpactForce = 100;

    [SerializeField] private GameObject weaponPickUpPrefab;
    [SerializeField] private Transform weaponHolder;

    [Header("Inventory")]

    [SerializeField] private int maxSlots = 2;
    [SerializeField] private List<Weapon> weaponSlots;
    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;


    void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();

        currentWeapon.bulletsInMagazine = currentWeapon.totalReserveAmmo;


    }



    void Update()
    {
        if (isShooting)
            Shoot();

    }

    #region Slot Managements / Weapon Ready

    public void SetDefaultWeapon(List<WeaponData> newWeaponData)
    {
        defaultWeaponData = new List<WeaponData>(newWeaponData);
        weaponSlots.Clear();

        foreach (WeaponData weaponData in defaultWeaponData)
        {
            PickUpWeapon(new Weapon(weaponData));
        }

        EquipWeapon(0);
    }
    private void EquipWeapon(int i)
    {
        if (i >= weaponSlots.Count)
            return;


        SetWeaponReady(false);

        currentWeapon = weaponSlots[i];


        player.visualController.PlayWeaponEquipAnimations();
        //CameraManager.instance.ChangeCameraDistance(currentWeapon.cameraDistance);

        UpdateWeaponUI();
    }


    public void PickUpWeapon(Weapon newWeapon)
    {


        if (WeaponInSlots(newWeapon.weaponType) != null)
        {
            WeaponInSlots(newWeapon.weaponType).totalReserveAmmo += newWeapon.bulletsInMagazine;
            return;
        }


        if (weaponSlots.Count >= maxSlots && newWeapon.weaponType != currentWeapon.weaponType)   //eğer izin verilen maks slot geçilirse return.
        {
            int weaponIndex = weaponSlots.IndexOf(currentWeapon);

            player.visualController.SwitchOffWeaponModels();

            weaponSlots[weaponIndex] = newWeapon;
            EquipWeapon(weaponIndex);

            return;
        }

        weaponSlots.Add(newWeapon); // değilse silahı slota ekle.
        player.visualController.SwitchOnBackupWeapons();

        UpdateWeaponUI();
    }
    private void DropWeapon()
    {
        if (HasOnlyOneWeapon())  // slotta 1 veya daha az silah varsa droplama.
            return;
        CreateWeaponOnTheGround();

        weaponSlots.Remove(currentWeapon); // elindeki silahı bırak.

        EquipWeapon(0); // elindeki silahı listede kalan silaha geçir.


    }

    private void CreateWeaponOnTheGround()
    {
        GameObject droppedWeapon = ObjectPool.instance.GetObject(weaponPickUpPrefab, transform);
        droppedWeapon.GetComponent<PickUp_Weapon>()?.setupPickUpWeapon(currentWeapon, transform);
    }

    public void SetWeaponReady(bool ready)
    {
        isWeaponReady = ready;

        if (ready)
            player.sound.weaponReady.Play();
    }
    public bool WeaponReady() => isWeaponReady;
    #endregion

    public void UpdateWeaponUI()
    {
        UI.instance.inGameUI.UpdateWeaponUI(weaponSlots, currentWeapon);
    }
    public Vector3 BulletDirection()
    {
        Transform aim = player.aim.Aim();

        Vector3 direction = (aim.position - GunPoint().position).normalized;

        if (player.aim.canAimPrecisely() == false)
            direction.y = 0f;



        return direction;

    }
    public bool HasOnlyOneWeapon() => weaponSlots.Count <= 1;

    public Weapon WeaponInSlots(WeaponType weaponType)
    {
        foreach (Weapon weapon in weaponSlots)
        {
            if (weapon.weaponType == weaponType)
                return weapon;
        }
        return null;
    }

    public bool HasWeaponTypeInInventory(WeaponType weaponType)
    {
        foreach (Weapon weapon in weaponSlots)
        {
            if (weapon.weaponType == weaponType)
                return true;
        }
        return false;
    }
    public Weapon CurrentWeapon() => currentWeapon;
    public Weapon SecondWeapon()
    {
        foreach (Weapon weapon in weaponSlots)
        {
            if (weapon != currentWeapon)
                return weapon;
        }

        return null;
    }
    public Transform GunPoint() => player.visualController.CurrentWeaponModel().gunPoint;

    private IEnumerator BurstFire()
    {
        SetWeaponReady(false);

        for (int i = 1; i <= currentWeapon.bulletsPerShot; i++)
        {
            FireSingleBullet();

            yield return new WaitForSeconds(currentWeapon.burstFireDelay);

            if (i >= currentWeapon.bulletsPerShot)
                SetWeaponReady(true);

        }
    }
    private void Shoot()
    {
        if (WeaponReady() == false)
            return;

        if (currentWeapon.canShoot() == false)
            return;

        player.visualController.PlayFireAnimation();

        if (currentWeapon.shootType == ShootType.Single)
            isShooting = false;

        if (currentWeapon.BurstActivated() == true)
        {
            StartCoroutine(BurstFire());
            return;

        }



        FireSingleBullet();
        TriggerEnemyDodge();
    }

    private void FireSingleBullet()
    {
        currentWeapon.bulletsInMagazine--;
        UpdateWeaponUI();

        float pitch = Random.Range(minPitch, maxPitch);
        player.visualController.CurrentWeaponModel().fireSFX.pitch = pitch;

        player.visualController.CurrentWeaponModel().fireSFX.Play();

        




        GameObject newBullet = ObjectPool.instance.GetObject(bulletPrefab, GunPoint());

        newBullet.transform.rotation = Quaternion.LookRotation(GunPoint().forward);

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        Bullet bulletScript = rbNewBullet.GetComponent<Bullet>();
        bulletScript.SetupBullet(whatIsAlly, currentWeapon.bulletDamage, currentWeapon.gunDistance, bulletImpactForce);

        Vector3 bulletsDirection = currentWeapon.ApplySpread(BulletDirection());

        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbNewBullet.velocity = bulletsDirection * bulletSpeed;
    }

    private void Reload()
    {
        SetWeaponReady(false);
        player.visualController.PlayReloadAnimation();
        player.visualController.CurrentWeaponModel().reloadSFX.Play();
        //burası sadece animasyon için asıl reload playeranimeventste.
    }

    private void TriggerEnemyDodge()
    {
        Vector3 rayOrigin = GunPoint().position;        // silahın ucundan mermi yönüne doğru bir raycast çizilir.
        Vector3 rayDirection = BulletDirection();

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, Mathf.Infinity))
        {
            Enemy_Melee enemy_Melee = hit.collider.gameObject.GetComponentInParent<Enemy_Melee>();

            if (enemy_Melee != null)
                enemy_Melee.ActivateDodgeRoll();
        }
    }

    #region Input Checks
    private void AssignInputEvents()
    {
        PlayerControls controls = player.controls;
        player.controls.Character.Fire.performed += context => isShooting = true;
        player.controls.Character.Fire.canceled += context => isShooting = false;

        controls.Character.EquipSlotOne.performed += context => EquipWeapon(0);
        controls.Character.EquipSlotTwo.performed += context => EquipWeapon(1);
        controls.Character.EquipSlotThree.performed += context => EquipWeapon(2);
        controls.Character.EquipSlotFour.performed += context => EquipWeapon(3);
        controls.Character.EquipSlotFive.performed += context => EquipWeapon(4);

        controls.Character.DropCurrentWeapon.performed += context => DropWeapon();

        controls.Character.Reload.performed += context =>
        {
            if (currentWeapon.canReload() && WeaponReady())
            {
                Reload();
            }
        };

        controls.Character.ToggleWeaponMode.performed += context => currentWeapon.ToggleBurst();

    }



    #endregion

}
