using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_WeaponSelection : MonoBehaviour
{
    [SerializeField] private GameObject nextUIToSwitchOn;
    public UI_SelectedWeaponWindow[] selectedWeapon;

    [Header("WarningInfo")]
    [SerializeField] private TextMeshProUGUI warningText;

    private float currentWarningAlpha;
    [SerializeField] private float dissAppearingSpeed = .25f;
    private float targetWarningAlpha;
    private void Start()
    {
        selectedWeapon = GetComponentsInChildren<UI_SelectedWeaponWindow>();
    }

    private void Update()
    {
        if (currentWarningAlpha > targetWarningAlpha)
        {
            currentWarningAlpha -= Time.deltaTime * dissAppearingSpeed;
            warningText.color = new Color(1, 1, 1, currentWarningAlpha);
        }
    }

    public void ConfirmWeaponSelection()
    {
         


        if (AtLeastOneWeaponSelected())
        {
            UI.instance.SwitchTo(nextUIToSwitchOn);
            UI.instance.StartLevelGeneration();
        }
        else
            ShowWarningMessage("En az bir silah seç.");
    }

    private bool AtLeastOneWeaponSelected() => SelectedWeaponData().Count > 0;
    public List<WeaponData> SelectedWeaponData()
    {
        List<WeaponData> selectedData = new List<WeaponData>();

        foreach (UI_SelectedWeaponWindow weapon in selectedWeapon)
        {
            if (weapon.weaponData != null)
                selectedData.Add(weapon.weaponData);
        }

        return selectedData;
    }


    public UI_SelectedWeaponWindow FindEmptySlot()
    {
        for (int i = 0; i < selectedWeapon.Length; i++)
        {
            if (selectedWeapon[i].isEmpty())
                return selectedWeapon[i];
        }

        return null;
    }

    public UI_SelectedWeaponWindow FindSlotWithWeaponOfType(WeaponData weaponData)
    {
        for (int i = 0; i < selectedWeapon.Length; i++)
        {
            if (selectedWeapon[i].weaponData == weaponData)
                return selectedWeapon[i];
        }

        return null;
    }

    public void ShowWarningMessage(string message)
    {
        warningText.color = Color.white;
        warningText.text = message;

        currentWarningAlpha = warningText.color.a;
        targetWarningAlpha = 0;
    }
}
