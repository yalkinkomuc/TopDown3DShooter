using UnityEngine;

public class EnemyBoss_Visuals : MonoBehaviour
{
    private Enemy_Boss enemy;

    [SerializeField] private float landingZoneOffset =1f;
    [SerializeField] private ParticleSystem landingZoneFX;

    [SerializeField] private GameObject[] batteries;
    [SerializeField] private GameObject[] weaponTrails;
    [SerializeField] private float initalBatteryScaleY = .2f;

    private float dischargeSpeed;
    private float rechargeSpeed;

    private bool isRecharching;
    private void Awake()
    {
        enemy = GetComponent<Enemy_Boss>();
        ResetBatteries();

        landingZoneFX.transform.parent = null;
        landingZoneFX.Stop();

    }

    private void Update()
    {
        UpdateBatteriesScale();
    }
    public void ResetBatteries()
    {
        isRecharching = true;

        rechargeSpeed = initalBatteryScaleY / enemy.abilityCooldown;
        dischargeSpeed = initalBatteryScaleY / (enemy.flameThrowDuration * .75f);

        foreach (GameObject battery in batteries)
        {
            battery.SetActive(true);
        }


    }
    private void UpdateBatteriesScale()
    {
        if (batteries.Length <= 0)
            return;

        foreach (GameObject battery in batteries)
        {
            if (battery.activeSelf)
            {
                float scaleChange = (isRecharching ? rechargeSpeed : -dischargeSpeed) * Time.deltaTime;
                float newScaleY =
                    Mathf.Clamp(battery.transform.localScale.y + scaleChange, 0, initalBatteryScaleY);

                battery.transform.localScale = new Vector3(0.15f, newScaleY, 0.15f);

                if (battery.transform.localScale.y <= 0)
                    battery.SetActive(false);

            }
        }

    }
    public void DischargeBatteries() => isRecharching = false;

    public void PlaceLandingZone(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        Vector3 offset = dir.normalized * landingZoneOffset;
        landingZoneFX.transform.position = target + offset;
        landingZoneFX.Clear();

        var mainModule = landingZoneFX.main;
        mainModule.startLifetime = enemy.travelTimeTarget * 2;

        landingZoneFX.Play();
    }

    public void EnableWeaponTrails(bool active)
    {

        if (weaponTrails.Length <= 0)
        {
            Debug.LogWarning("No Weapon Trails Assigned");
            return;
        }

        foreach (var trail in weaponTrails)
        {
            trail.gameObject.SetActive(active);
        }
    }
}
