using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_WeaponSelectionButton : UI_Button

{
    private UI_WeaponSelection weaponSelectionUI;

    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Image weaponIcon;

    private UI_SelectedWeaponWindow emptySlot;


    private void OnValidate()
    {
        gameObject.name = "Button - Select Weapon:" + weaponData.weaponType;
    }
    public override void Start()
    {
        base.Start();

        weaponSelectionUI = GetComponentInParent<UI_WeaponSelection>();

        weaponIcon.sprite = weaponData.weaponIcon;
    }


    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        weaponIcon.color = Color.yellow;

        emptySlot = weaponSelectionUI.FindEmptySlot();
        emptySlot?.UpdateSlotInfo(weaponData);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        weaponIcon.color = Color.white;

        bool noMoreEmptySlots = weaponSelectionUI.FindEmptySlot() == null;
        bool noThisWeaponInSlots = weaponSelectionUI.FindSlotWithWeaponOfType(weaponData) == null;  

        if(noMoreEmptySlots && noThisWeaponInSlots)
        {
            weaponSelectionUI.ShowWarningMessage("Boş yer yok. Çıkarmak istediğiniz silahı seçin.");
            return;
        }

        UI_SelectedWeaponWindow slotBusyWithThisWeapon = weaponSelectionUI.FindSlotWithWeaponOfType(weaponData);

        if (slotBusyWithThisWeapon != null)
        {
            slotBusyWithThisWeapon.SetWeaponSlot(null);

        }

        else
        {
            emptySlot = weaponSelectionUI.FindEmptySlot();
            emptySlot.SetWeaponSlot(weaponData);
        }



        emptySlot = null;

    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        weaponIcon.color = Color.white;
        emptySlot?.UpdateSlotInfo(null);
        emptySlot = null;
    }

}
