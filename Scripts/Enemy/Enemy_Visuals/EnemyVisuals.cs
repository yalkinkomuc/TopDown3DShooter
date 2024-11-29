using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public enum Enemy_Melee_WeaponType { OneHand, Throwable, Unarmed }
public enum Enemy_Range_WeaponType { Pistol, Revolver, Shotgun, Rifle, Sniper,Random }

public class EnemyVisuals : MonoBehaviour
{
    [Header("CorruptionCrystals")]
    [SerializeField] private GameObject[] corruptionCrystals;
    [SerializeField] private int corruptionAmount;

    [Header("Color")]
    [SerializeField] private Texture[] textures;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    [SerializeField] private Transform LeftHandIK;
    [SerializeField] private Transform LeftElbowIK;
    [SerializeField] private TwoBoneIKConstraint leftHandIKConstraint;
    [SerializeField] private MultiAimConstraint weaponAimConstraint;

    public GameObject grenadeModel;
    [SerializeField] private Rig rig;

    public GameObject currentWeaponModel { get; private set; }

    private float leftHandTargetWeight;
    private float weaponAimTargetWeight;
    private float rigChangeRate;


    private void Update()
    {
        if (leftHandIKConstraint != null)
            leftHandIKConstraint.weight = AdjustIKWeight(leftHandIKConstraint.weight, leftHandTargetWeight);

        if (weaponAimConstraint != null)
            weaponAimConstraint.weight = AdjustIKWeight(weaponAimConstraint.weight, weaponAimTargetWeight);
    }

    public void EnableWeaponTrail(bool enable)
    {
        Enemy_Weapon_Model currentWeaponScripts = currentWeaponModel.GetComponent<Enemy_Weapon_Model>();

        currentWeaponScripts.EnableTrailEffects(enable);


    }

    public void EnableGrenadeModel(bool active)
    {
        grenadeModel?.SetActive(active);
    }
    public void SetupRandomLook()
    {
        ChangeRandomMaterial();
        SetupRandomWeaponModel();
        SetupRandomCorruption();
    }
    private void SetupRandomCorruption()
    {
        List<int> availableIndexs = new List<int>();
        corruptionCrystals = CollectCorruptionCrystals();


        for (int i = 0; i < corruptionCrystals.Length; i++)
        {
            availableIndexs.Add(i);
            corruptionCrystals[i].SetActive(false);
        }

        corruptionAmount = Random.Range(0, corruptionCrystals.Length);

        for (int i = 0; i < corruptionAmount; i++)
        {
            if (availableIndexs.Count == 0)
                break;

            int randomIndex = Random.Range(0, availableIndexs.Count);
            int objectIndex = availableIndexs[randomIndex];

            corruptionCrystals[objectIndex].SetActive(true);
            availableIndexs.RemoveAt(randomIndex);
        }

    }
    private void SetupRandomWeaponModel()
    {
        bool thisEnemyIsMelee = GetComponent<Enemy_Melee>() != null;
        bool thisEnemyIsRange = GetComponent<Enemy_Range>() != null;

        if (thisEnemyIsMelee)
            currentWeaponModel = FindMeleeWeaponModel();

        if (thisEnemyIsRange)
            currentWeaponModel = FindRangeWeaponModel();

        currentWeaponModel.SetActive(true);

        OverrideAnimControllerIfPoss();

    }
    private void ChangeRandomMaterial()
    {

        int randomIndex = Random.Range(0, textures.Length);

        Material newMat = new Material(skinnedMeshRenderer.material);

        newMat.mainTexture = textures[randomIndex];

        skinnedMeshRenderer.material = newMat;


    }

    private void LeftHandIKSetup(Transform leftHand, Transform LeftElbow)
    {
        LeftHandIK.localPosition = leftHand.localPosition;
        LeftHandIK.localRotation = leftHand.localRotation;

        LeftElbowIK.localPosition = LeftElbow.localPosition;
        LeftElbowIK.localRotation = LeftElbow.localRotation;
    }


    private GameObject FindRangeWeaponModel()
    {
        Enemy_Range_WeaponModel[] weaponModels = GetComponentsInChildren<Enemy_Range_WeaponModel>(true);
        Enemy_Range_WeaponType weaponType = GetComponent<Enemy_Range>().weaponType;

        foreach (var weaponModel in weaponModels)
        {
            if (weaponModel.weaponType == weaponType)
            {

                SwitchAnimationLayer(((int)weaponModel.weaponHoldType));
                LeftHandIKSetup(weaponModel.leftHandTarget, weaponModel.leftElbowTarget);
                return weaponModel.gameObject;

            }



        }
        //olmazsa buraya debug warning at
        return null;
    }

    private GameObject FindSecondaryWeaponModel()
    {
        Enemy_SecondaryRangeWeaponModel[] weaponModels = GetComponentsInChildren<Enemy_SecondaryRangeWeaponModel>(true);
        Enemy_Range_WeaponType weaponType = GetComponentInParent<Enemy_Range>().weaponType;

        foreach (var weaponModel in weaponModels)
        {
            if (weaponModel.weaponType == weaponType)
                return weaponModel.gameObject;
        }
        return null;
    }


    public void EnableIK(bool enableLeftHand, bool enableAim, float changeRate = 10)
    {
        if (leftHandIKConstraint == null)
        {
            Debug.LogWarning("No IK assigned");
            return;
        }


        rigChangeRate = changeRate;
        leftHandTargetWeight = enableLeftHand ? 1 : 0;
        weaponAimTargetWeight = enableAim ? 1 : 0;



    }

    private float AdjustIKWeight(float currentWeight, float targetWeight)
    {
        if (Mathf.Abs(currentWeight - targetWeight) > 0.05f)
            return Mathf.Lerp(currentWeight, targetWeight, rigChangeRate * Time.deltaTime);
        else
            return targetWeight;



    }
    public void EnableWeaponModel(bool active)
    {
        currentWeaponModel?.gameObject.SetActive(active);
    }

    public void EnableSecondaryWeaponModel(bool active)
    {
        FindSecondaryWeaponModel().SetActive(active);
    }

    private GameObject FindMeleeWeaponModel()
    {
        Enemy_Weapon_Model[] weaponModels = GetComponentsInChildren<Enemy_Weapon_Model>(true);

        Enemy_Melee_WeaponType weaponType = GetComponent<Enemy_Melee>().weaponModelType;

        List<Enemy_Weapon_Model> filteredWeaponModels = new List<Enemy_Weapon_Model>();

        foreach (var weaponModel in weaponModels)
        {
            if (weaponModel.weaponModelType == weaponType)
                filteredWeaponModels.Add(weaponModel);
        }

        int randomIndex = Random.Range(0, filteredWeaponModels.Count);

        return filteredWeaponModels[randomIndex].gameObject;
    }

    private void OverrideAnimControllerIfPoss()
    {
        AnimatorOverrideController overrideController = currentWeaponModel.GetComponent<Enemy_Weapon_Model>()?.overrideController;

        if (overrideController != null)
        {
            GetComponentInChildren<Animator>().runtimeAnimatorController = overrideController;
        }
    }
    private void SwitchAnimationLayer(int layerIndex)
    {

        Animator anim = GetComponentInChildren<Animator>();

        for (int i = 1; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0);
        }

        anim.SetLayerWeight(layerIndex, 1);
    }

    private GameObject[] CollectCorruptionCrystals()
    {
        EnemyCorruptionCrystal[] crystalComponents = GetComponentsInChildren<EnemyCorruptionCrystal>(true);
        GameObject[] corruptionCrystals = new GameObject[crystalComponents.Length];

        for (int i = 0; i < crystalComponents.Length; i++)
        {
            corruptionCrystals[i] = crystalComponents[i].gameObject;
        }

        return corruptionCrystals;
    }
}
