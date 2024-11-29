using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Defense Mission ", menuName = "Missions/Defense")]
public class MissionLastDefense : Mission
{
    public bool defenseBegun = false;

    [Header("Cooldown and Duration")]
    public float defenseDuration = 120;
    private float defenseTimer;
    public float waveCooldown = 15;
    private float waveTimer;

    [Header("Respawn Details")]
    public int amountOfRespawnPoints = 2;
    public List<Transform> respawnPoints;
    private Vector3 defensePoint;
    [Space]

    public int enemiesPerWave;
    public GameObject[] possibleEnemies;

    private string defenseTimerText;

    private void OnEnable()
    {
        defenseBegun = false;
    }

    public override void StartMission()
    {
        defensePoint = FindObjectOfType<MissionEndTrigger>().transform.position;
        respawnPoints = new List<Transform>(ClosestPoints(amountOfRespawnPoints));

        UI.instance.inGameUI.UpdateMissionInfo("Tahliye noktasına git.");
    }



    public override bool MissionCompleted()
    {
        if (defenseBegun == false)
        {
            StartDefenseEvent();
            return false;


        }

        return defenseTimer < 0;
    }

    public override void UpdateMission()
    {
        if (defenseBegun == false)
            return;

        waveTimer -= Time.deltaTime;
        if (defenseTimer > 0)
            defenseTimer -= Time.deltaTime;

        if (waveTimer < 0)
        {
            CreateNewEnemies(enemiesPerWave);
            waveTimer = waveCooldown;
        }

        defenseTimerText = System.TimeSpan.FromSeconds(defenseTimer).ToString("mm':'ss");

        string missionText = "Uçak kalkışa geçene kadar hayatta kal!";
        string missionDetails = "Kalan zaman: " + defenseTimerText;

        UI.instance.inGameUI.UpdateMissionInfo(missionText, missionDetails);

    }

    private void StartDefenseEvent()
    {
        waveTimer = .5f;
        defenseTimer = defenseDuration;
        defenseBegun = true;
    }

    private void CreateNewEnemies(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int randomEnemyIndex = Random.Range(0, possibleEnemies.Length);
            int randomRespawnIndex = Random.Range(0, respawnPoints.Count);


            Transform randomRespawnPoint = respawnPoints[randomRespawnIndex];
            GameObject randomEnemy = possibleEnemies[randomEnemyIndex];

            randomEnemy.GetComponent<Enemy>().aggresionRange = 100;

            ObjectPool.instance.GetObject(randomEnemy, randomRespawnPoint);
        }

    }

    private List<Transform> ClosestPoints(int amount)
    {
        List<Transform> closestPoints = new List<Transform>();
        List<MissionObjectEnemyRespawnPoint> allpoints =
            new List<MissionObjectEnemyRespawnPoint>(FindObjectsOfType<MissionObjectEnemyRespawnPoint>());

        while (closestPoints.Count < amount && allpoints.Count > 0)
        {
            float shortestDistance = float.MaxValue;
            MissionObjectEnemyRespawnPoint closestPoint = null;

            foreach (var point in allpoints)
            {
                float distance = Vector3.Distance(point.transform.position, defensePoint);

                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestPoint = point;
                }
            }

            if (closestPoint != null)
            {
                closestPoints.Add(closestPoint.transform);
                allpoints.Remove(closestPoint);
            }

        }


        return closestPoints;
    }


}
