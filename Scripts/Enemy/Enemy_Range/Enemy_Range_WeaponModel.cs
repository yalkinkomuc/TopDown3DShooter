using UnityEngine;
using UnityEngine.Animations.Rigging;


public enum holdType_EnemyRange { Common, LowHold, HighHold }

public class Enemy_Range_WeaponModel : MonoBehaviour
{
    public Transform gunPoint;
    [Space]

    public Enemy_Range_WeaponType weaponType;
    public holdType_EnemyRange weaponHoldType;

    public Transform leftHandTarget;
    public Transform leftElbowTarget;

    
}
