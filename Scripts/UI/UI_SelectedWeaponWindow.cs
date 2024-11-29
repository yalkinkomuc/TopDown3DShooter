using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectedWeaponWindow : MonoBehaviour
{

    public WeaponData weaponData;

    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI weaponName;


    private void Start()
    {
        weaponData = null;
        UpdateSlotInfo(null);
    }

    public void SetWeaponSlot(WeaponData newWeaponData)
    {
        weaponData = newWeaponData; 
        UpdateSlotInfo(newWeaponData);
    }

    public void UpdateSlotInfo(WeaponData weaponData)
    {
        if (weaponData == null)
        {
            weaponIcon.color = Color.clear;
            weaponName.text = "Bir silah seçin.";
            return;
        }

        weaponIcon.color = Color.white;
        weaponIcon.sprite = weaponData.weaponIcon;
        weaponName.text = weaponData.WeaponName;

    }
    
    public bool isEmpty() => weaponData == null;
}
