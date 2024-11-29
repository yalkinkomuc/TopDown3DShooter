using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Weapon_Model : MonoBehaviour
{
    public Enemy_Melee_WeaponType weaponModelType;
    public AnimatorOverrideController overrideController;
    public EnemyMelee_WeaponData weaponData;
    [SerializeField] private GameObject[] trailEffects;

    [Header("Damage Attributes")]

    public Transform[] damagePoints;
    public float attackRadius;

    [ContextMenu("Assign damage point transforms")]

    private void GetDamagePoints()
    {
        damagePoints = new Transform[trailEffects.Length];   // damage pointlerle trail effectleri eşitliyoruz.
        for (int i = 0; i < trailEffects.Length; i++)
        {
            damagePoints[i] = trailEffects[i].transform;
        }
    }

    public void EnableTrailEffects(bool enable)
    {
        foreach (var effect in trailEffects)
        {
            effect.SetActive(enable);
        }


    }

    private void OnDrawGizmos()
    {
        if(damagePoints.Length > 0)
        {
            foreach (Transform point  in damagePoints)
            {
                Gizmos.DrawWireSphere(point.position, attackRadius);
            }
        }
    }

}
