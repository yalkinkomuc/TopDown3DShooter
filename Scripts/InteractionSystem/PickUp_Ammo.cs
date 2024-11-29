using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AmmoData
{
    public WeaponType weaponType;
    [Range(10, 100)] public int minAmount;
    [Range(10, 100)] public int maxAmount;
}

public enum AmmoBoxType { smallBox, largeBox }

public class PickUp_Ammo : Interactable
{
    [SerializeField] private AmmoBoxType ammoBoxType;

    

    [SerializeField] private List<AmmoData> smallBoxAmmo;
    [SerializeField] private List<AmmoData> largeBoxAmmo;

    [SerializeField] private GameObject[] boxModel;

    private void Start()
    {
        SetupBoxModel();
    }

    private void SetupBoxModel()
    {
        for (int i = 0; i < boxModel.Length; i++)
        {

            boxModel[i].SetActive(false);

            if (i == ((int)ammoBoxType))
            {
                boxModel[i].SetActive(true);
                UpdateMeshMaterial(boxModel[i].GetComponent<MeshRenderer>());
            }
        }
    }
    public override void Interaction()
    {
        List<AmmoData> currentAmmoList = smallBoxAmmo;

        if (ammoBoxType == AmmoBoxType.largeBox)
            currentAmmoList = largeBoxAmmo;

        foreach (AmmoData ammo in currentAmmoList)
        {
            Weapon weapon = weaponController.WeaponInSlots(ammo.weaponType);

            AddBulletsToWeapon(weapon, GetBulletAmount(ammo));
        }

        ObjectPool.instance.ReturnObject(gameObject);

    }

    
    private int GetBulletAmount(AmmoData ammoData)
    {
        float min = Mathf.Min(ammoData.minAmount, ammoData.maxAmount);
        float max = Mathf.Max(ammoData.minAmount, ammoData.maxAmount);

        float randomAmmoAmount = Random.Range(min, max);
        return Mathf.RoundToInt(randomAmmoAmount);
    }
    private void AddBulletsToWeapon(Weapon weapon, int amount)
    {
        if (weapon == null)
            return;

        weapon.totalReserveAmmo += amount;
    }

}
