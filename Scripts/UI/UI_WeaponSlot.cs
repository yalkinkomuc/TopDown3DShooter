using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_WeaponSlot : MonoBehaviour
{
    public Image weaponIcon;
    public TextMeshProUGUI ammoText;

    private void Awake()
    {
        weaponIcon = GetComponentInChildren<Image>();
        ammoText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void UpdateWeaponSlot(Weapon myWeapon, bool activeWeapon)
    {
        if (myWeapon == null)
        {
            weaponIcon.color = Color.clear;
            ammoText.text = "";
            return;
        }

        Color newColor = activeWeapon ? Color.white : new Color(1f, 1f, 1f, .222f);

        weaponIcon.color = newColor;
        weaponIcon.sprite = myWeapon.WeaponData.weaponIcon;

        ammoText.text = myWeapon.bulletsInMagazine + "/" + myWeapon.totalReserveAmmo;
        ammoText.color = Color.white;

    }
}
