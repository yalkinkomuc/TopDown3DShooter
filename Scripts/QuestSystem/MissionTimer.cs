using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Timer Mission ", menuName = "Missions/Timer mission")]

public class MissionTimer : Mission
{
    public float time;
    private float currentTime;
    public override void StartMission()
    {
        currentTime = time;
    }

    public override void UpdateMission()
    {
        currentTime -= Time.deltaTime;

        //if (currentTime <= 0)
        //{
            //GameManager.instance.GameOver(); // yoruma alınabilir.
        //}

        string timeText = System.TimeSpan.FromSeconds(currentTime).ToString("mm':'ss");

        

        string missionText = "Uçak kalkışa geçene kadar tahliye noktasına git!";
        string missionDetails = "Kalan zaman: " + timeText;

        UI.instance.inGameUI.UpdateMissionInfo(missionText, missionDetails);
    }


    public override bool MissionCompleted()
    {
        return currentTime > 0;
    }
}
