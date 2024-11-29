using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp_Weapon : Interactable
{
    
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Weapon weapon;
    [SerializeField] private BackupWeaponModel[] models;

    private bool oldWeapon;

    private void Start()
    {

        if(oldWeapon==false)
            weapon = new Weapon(weaponData);

       SetupGameObject();
    }

    public void setupPickUpWeapon(Weapon weapon,Transform transform)
    {
        oldWeapon = true;

        this.weapon = weapon;
        weaponData = weapon.WeaponData;

        this.transform.position = transform.position + new Vector3 (0f, 0.75f, 0f);
    }

    [ContextMenu ("Update Item Model")]
    public void SetupGameObject()
    {
        gameObject.name = "PickUp Weapon - " + weaponData.weaponType.ToString();
        SetupWeaponModel();
    }
    
    private void SetupWeaponModel()
    {
        foreach (BackupWeaponModel model in models)
        { 
            model.gameObject.SetActive(false);

            if (model.weaponType == weaponData.weaponType)
            {
                model.gameObject.SetActive(true);
                UpdateMeshMaterial(model.GetComponent<MeshRenderer>());
            }
        }

    }

    public override void Interaction()
    {
        weaponController.PickUpWeapon(weapon);
        ObjectPool.instance.ReturnObject(gameObject);
    }

   

}
