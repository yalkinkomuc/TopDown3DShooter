
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private WeaponVisualController WeaponVisualController;
    private PlayerWeaponController playerWeaponController;
    private void Start()
    {
        WeaponVisualController = GetComponentInParent<WeaponVisualController>();
        playerWeaponController = GetComponentInParent<PlayerWeaponController>();
    }

    public void reloadIsOver()
    {
        WeaponVisualController.MaximizeRigWeight();
        playerWeaponController.CurrentWeapon().RefillAmmo();

        WeaponVisualController.CurrentWeaponModel().reloadSFX.Stop();

        playerWeaponController.SetWeaponReady(true);
        playerWeaponController.UpdateWeaponUI();
    }

    public void returnRig()
    {
        WeaponVisualController.MaximizeRigWeight();
        WeaponVisualController.MaximizeLeftHandIKWeight();
    }

    public void gunEquipingIsOver()
    {
        playerWeaponController.SetWeaponReady(true);
    }

    public void SwitchOnWeaponModel() => WeaponVisualController.SwitchOnCurrentWeaponModel();
}
