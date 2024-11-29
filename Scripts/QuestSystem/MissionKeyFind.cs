using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = " Key Mission ", menuName = "Missions/Key Mission")]
public class MissionKeyFind : Mission
{
    [SerializeField] private GameObject key;
    private bool keyFound = false;

    public override void StartMission()
    {
        MissionObject_Key.OnKeyPickedUp += PickUpKey;

        UI.instance.inGameUI.UpdateMissionInfo("Anahtara sahip düşmanı bul ve anahtarı ele geçir.");

        Debug.Log("Mission Begun");
        Enemy enemy = LevelGenerator.instance.GetRandomEnemy();
        enemy.GetComponent<EnemyDropController>()?.GiveKey(key);
        enemy.MakeEnemyVIP();

    }
    public override bool MissionCompleted()
    {
        return keyFound;
    }

    private void PickUpKey()
    {
        keyFound = true;
        MissionObject_Key.OnKeyPickedUp -= PickUpKey;

        UI.instance.inGameUI.UpdateMissionInfo("Anahtarı buldun! Tahliye noktasına git.");


    }

}
    

