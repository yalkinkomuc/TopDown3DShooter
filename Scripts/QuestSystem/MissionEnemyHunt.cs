using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Hunt-Mission ", menuName = "Missions/Hunt Mission")]

public class MissionEnemyHunt : Mission
{
    public int amountToKill = 12;
    public EnemyType enemyType;

    private int targetsToKillLeft;
    public override void StartMission()
    {

        targetsToKillLeft = amountToKill;
        UpdateMissionUI();
        MissionObjectHuntTarget.onTargetKilled += ReducuKillTargetsAmount;

        List<Enemy> validEnemies = new List<Enemy>();

        if (enemyType == EnemyType.Random)
            validEnemies = LevelGenerator.instance.GetEnemyList();
        else
        {
            foreach (Enemy enemy in LevelGenerator.instance.GetEnemyList())
            {
                if (enemy.enemyType == enemyType)
                    validEnemies.Add(enemy);
            }
        }


        for (int i = 0; i < amountToKill; i++)
        {
            if (validEnemies.Count <= 0)
                return;

            int randomIndex = Random.Range(0, validEnemies.Count);
            validEnemies[randomIndex].AddComponent<MissionObjectHuntTarget>();
            validEnemies.RemoveAt(randomIndex);
        }

    }
    public override bool MissionCompleted()
    {
        return targetsToKillLeft <= 0;
    }

    private void ReducuKillTargetsAmount()
    {
        targetsToKillLeft--;

        UpdateMissionUI();


        if (targetsToKillLeft <= 0)
        {
            UI.instance.inGameUI.UpdateMissionInfo("Tahliye noktasına git.",null);
            MissionObjectHuntTarget.onTargetKilled -= ReducuKillTargetsAmount;
        }
    }

    private void UpdateMissionUI()
    {
        string missionText = "Sinyal bozucuya sahip " + amountToKill + " düşmanı öldür. Bu sinyal bozucular uçağın radar sistemini bozarak uçuşa engel oluyor!";
        string missionDetails = "Kalan Düşmanlar : " + targetsToKillLeft;

        UI.instance.inGameUI.UpdateMissionInfo(missionText, missionDetails);
    }
}
