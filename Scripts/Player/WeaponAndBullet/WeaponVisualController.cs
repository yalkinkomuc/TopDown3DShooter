using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponVisualController : MonoBehaviour
{
    private Player player;
    private Animator anim;






    [SerializeField] private WeaponModel[] weaponModels;
    [SerializeField] private BackupWeaponModel[] BackupWeaponModels;

    [Header("Rig")]
    [SerializeField] private float rigWeightIncreaseRate;
    private bool shouldIncrease_RigRate;
    private Rig rig;


    [Header("Left Hand IK")]
    [SerializeField] TwoBoneIKConstraint leftHandIK;
    [SerializeField] private Transform leftHandIK_Target;
    [SerializeField] private float leftHandIKIncreaseRate;
    private bool shouldIncrease_LeftHandIKWeight;




    private void Start()
    {
        player = GetComponent<Player>();
        anim = GetComponentInChildren<Animator>();
        rig = GetComponentInChildren<Rig>();

        weaponModels = GetComponentsInChildren<WeaponModel>(true);
        BackupWeaponModels = GetComponentsInChildren<BackupWeaponModel>(true);



    }
    private void Update()
    {

        UpdateRigWeight();
        UpdateLeftHandIKWeight();

    }

    public void PlayFireAnimation() => anim.SetTrigger("Fire");

    public WeaponModel CurrentWeaponModel()
    {
        WeaponModel weaponModel = null;

        WeaponType weaponType = player.weaponController.CurrentWeapon().weaponType;

        for (int i = 0; i < weaponModels.Length; i++)
        {
            if (weaponModels[i].weaponType == weaponType)
                weaponModel = weaponModels[i];
        }

        return weaponModel;
    }

    public void PlayReloadAnimation()
    {
        float reloadSpeed = player.weaponController.CurrentWeapon().reloadSpeed;



        anim.SetFloat("ReloadSpeed", reloadSpeed);
        anim.SetTrigger("Reload");
        ReduceRigWeight();


    }

    #region Rig Settings
    private void UpdateLeftHandIKWeight()
    {
        if (shouldIncrease_LeftHandIKWeight)
        {
            leftHandIK.weight += leftHandIKIncreaseRate * Time.deltaTime;
            if (rig.weight >= 1)
                shouldIncrease_LeftHandIKWeight = false;
        }
    }
    private void UpdateRigWeight()
    {
        if (shouldIncrease_RigRate)
        {
            rig.weight += rigWeightIncreaseRate * Time.deltaTime;
            if (rig.weight >= 1)
                shouldIncrease_RigRate = false;
        }
    }
    private void ReduceRigWeight()
    {
        rig.weight = 0;
    }
    public void MaximizeRigWeight() => shouldIncrease_RigRate = true;
    public void MaximizeLeftHandIKWeight() => shouldIncrease_LeftHandIKWeight = true;
    private void attachLeftHand()
    {
        Transform targetTransform = CurrentWeaponModel().holdPoint;

        leftHandIK_Target.localPosition = targetTransform.localPosition;
        leftHandIK_Target.localRotation = targetTransform.localRotation;
    }
    #endregion

    public void PlayWeaponEquipAnimations()
    {


        EquipType equipType = CurrentWeaponModel().equipAnimationType;

        float equipmentSpeed = player.weaponController.CurrentWeapon().equipmentSpeed;
        leftHandIK.weight = 0;
        ReduceRigWeight();
        anim.SetFloat("GunEquipType", ((float)equipType));
        anim.SetTrigger("EquipWeapon");
        anim.SetFloat("EquipSpeed", equipmentSpeed);

    }








    public void SwitchOnCurrentWeaponModel()
    {
        int animationIndex = ((int)CurrentWeaponModel().holdType);

        SwitchOffWeaponModels();

        SwitchOffBackupWeapons();


        if (player.weaponController.HasOnlyOneWeapon() == false)
            SwitchOnBackupWeapons();

        SwitchAnimationLayer(animationIndex);

        CurrentWeaponModel().gameObject.SetActive(true);

        attachLeftHand();
    }

    public void SwitchOffBackupWeapons()
    {
        foreach (BackupWeaponModel backupWeaponModel in BackupWeaponModels)
        {
            backupWeaponModel.Activate(false);
        }
    }
    public void SwitchOnBackupWeapons()
    {
        SwitchOffBackupWeapons();

        BackupWeaponModel lowHangWeapon = null;
        BackupWeaponModel backHangWeapon = null;
        BackupWeaponModel sideHangWeapon = null;

        foreach (BackupWeaponModel backupWeaponModel in BackupWeaponModels)
        {
            if (backupWeaponModel.weaponType == player.weaponController.CurrentWeapon().weaponType)
                continue;


            if (player.weaponController.HasWeaponTypeInInventory(backupWeaponModel.weaponType))
            {
                if (backupWeaponModel.HangTypeIs(HangType.lowBackHang))
                    lowHangWeapon = backupWeaponModel;

                if (backupWeaponModel.HangTypeIs(HangType.BackHang))
                    backHangWeapon = backupWeaponModel;

                if (backupWeaponModel.HangTypeIs(HangType.SideHang))
                    sideHangWeapon = backupWeaponModel;

            }
        }
        lowHangWeapon?.Activate(true);
        backHangWeapon?.Activate(true);
        sideHangWeapon?.Activate(true);


    }

    public void SwitchOffWeaponModels()
    {
        for (int i = 0; i < weaponModels.Length; i++)
        {
            weaponModels[i].gameObject.SetActive(false);
        }
    }


    private void SwitchAnimationLayer(int layerIndex)
    {
        for (int i = 1; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0);
        }

        anim.SetLayerWeight(layerIndex, 1);
    }
}

