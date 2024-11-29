using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_MissionSelectionButton : UI_Button
{
    private UI_MissionSelection missionUI;

    [SerializeField] private Mission myMission;
    private TextMeshProUGUI myText;

    private void OnValidate()
    {
        gameObject.name = "Button - Select Mission: " + myMission.MissionName;
        
    }

    public override void Start()
    {
        base.Start();

        missionUI = GetComponentInParent<UI_MissionSelection>();
        myText = GetComponentInChildren<TextMeshProUGUI>();
        myText.text = myMission.MissionName;

    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        missionUI.UpdateMissionDescription(myMission.MissionDescription);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        missionUI.UpdateMissionDescription("Bir görev seçin. ");
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        MissionManager.instance.SetCurrentMission(myMission);

        
    }
}
