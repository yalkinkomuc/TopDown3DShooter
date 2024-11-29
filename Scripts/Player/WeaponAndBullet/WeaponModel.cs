
using UnityEngine;


public enum EquipType { sideEquipAnimation, backEquipAnimatio };
public enum holdType { commonHold =1 , LowHold , HighHold };
public class WeaponModel : MonoBehaviour
{
    public WeaponType weaponType;
    public EquipType equipAnimationType;
    public holdType holdType;

    public Transform gunPoint;
    public Transform holdPoint;

    public AudioSource fireSFX;
    public AudioSource reloadSFX;
}
