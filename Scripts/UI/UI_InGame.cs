using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{

    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject carUI;

    [Header("HealthBar")]
    [SerializeField] Image healthBar;

    [Header("Weapon")]
    [SerializeField] private UI_WeaponSlot[] weaponSlotsUI;

    [Header("Missions")]
    [SerializeField] private GameObject missionToolTipParent;
    [SerializeField] private GameObject missionHelpToolTip;
    private bool toolTipActive = true;
    [SerializeField] private TextMeshProUGUI missionText;
    [SerializeField] private TextMeshProUGUI missionDetails;

    [Header("CarInfo")]
    [SerializeField] private Image carHealthBar;
    [SerializeField] private TextMeshProUGUI carSpeedText;
        
    private void Awake()
    {
        weaponSlotsUI = GetComponentsInChildren<UI_WeaponSlot>();

    }

    private void Start()
    {
        SwitchMissionToolTip();
    }

    public void UpdateMissionInfo(string missionText,string missionDetails = "")
    {
        this.missionText.text = missionText;
        this.missionDetails.text = missionDetails;
    }

    public void UpdateWeaponUI (List<Weapon> weaponSlots,Weapon currentWeapon)
    {
        for (int i = 0; i < weaponSlotsUI.Length; i++)
        {
            if (i < weaponSlots.Count)
            {
                bool isActiveWeapon = weaponSlots[i] == currentWeapon ? true : false;
                weaponSlotsUI[i].UpdateWeaponSlot(weaponSlots[i],isActiveWeapon);
            }
            else
            {
                weaponSlotsUI[i].UpdateWeaponSlot(null,false);
            }
        }
    }

    public void SwitchToCharacterUI()
    {
        characterUI.SetActive(true);
        carUI.SetActive(false);
    }

    public void SwitchToCarUI()
    {
        characterUI.SetActive(false );
        carUI.SetActive(true);
    }

    public void SwitchMissionToolTip()
    {
        toolTipActive = !toolTipActive;
        missionToolTipParent.SetActive(toolTipActive);
        missionHelpToolTip.SetActive(!toolTipActive);
    }

    public void UpdateHealthUI (float currentHealth, float maxHealth)
    {
        healthBar.fillAmount = currentHealth /maxHealth;  // canın resminin dolma oranını can yüzdesine eşitliyoruz.
    }

    public void UpdateCarHealthUI(float currentCarHealth, float maxCarHealth)
    {
        carHealthBar.fillAmount = currentCarHealth /maxCarHealth;
    }

    public void UpdateSpeedText(string text) => carSpeedText.text = text;
}
