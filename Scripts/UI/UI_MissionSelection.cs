using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_MissionSelection : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI missionDescription;

    public void UpdateMissionDescription(string text)
    {
        missionDescription.text = text;
    }
}
