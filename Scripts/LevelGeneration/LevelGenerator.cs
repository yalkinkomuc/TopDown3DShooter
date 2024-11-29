using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator instance;

    [SerializeField] private List<Enemy> enemyList;

    [SerializeField] private NavMeshSurface navMeshSurface;
    [Space]
    [SerializeField] private Transform lastLevelPart;
    [SerializeField] private List<Transform> levelParts;
    private List<Transform> currentLevelParts;
    private List<Transform> generatedLevelParts = new List<Transform> ();
    [SerializeField] private SnapPoint nextSnapPoint;
    private SnapPoint defaultSnapPoint;

    [Space]
    [SerializeField] private float generationCooldown;
    private float cooldownTimer;
    private bool generationOver = true;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        enemyList = new List<Enemy>();
        defaultSnapPoint = nextSnapPoint;
    }

    private void Update()
    {

        if (generationOver)
            return;
        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer < 0)
        {

            if (currentLevelParts.Count > 0)
            {
                cooldownTimer = generationCooldown;
                GenerateNextLevelPart();

            }
            else if (generationOver == false)
            {
                FinishGeneration();
            }

        }
    }

    [ContextMenu("Restart Generation")]
    public void InitializieGeneration()
    {
        nextSnapPoint = defaultSnapPoint;
        generationOver = false;
        currentLevelParts = new List<Transform>(levelParts);
        DestroyOldLevelPartsAndEnemies();

    }

    private void DestroyOldLevelPartsAndEnemies()
    {
        foreach (Enemy enemy in enemyList)
        {
            Destroy(enemy.gameObject);

        }


        foreach (Transform t in generatedLevelParts)
        {
            Destroy(t.gameObject);
        }

        generatedLevelParts = new List<Transform>();
        enemyList = new List<Enemy>();
    }

    private void FinishGeneration()
    {
        generationOver = true;

        GenerateNextLevelPart();

        navMeshSurface.BuildNavMesh();

        foreach (Enemy enemy in enemyList)
        {
            enemy.transform.parent = null;
            enemy.gameObject.SetActive(true);
        }

        MissionManager.instance.StartMission();
    }



    [ContextMenu("Create next level part")]
    private void GenerateNextLevelPart()
    {

        Transform newPart = null;

        if (generationOver)
            newPart = Instantiate(lastLevelPart);
        else
            newPart = Instantiate(ChooseRandomPart());

        generatedLevelParts.Add(newPart);

        LevelParts levelPartScript = newPart.GetComponent<LevelParts>();

        levelPartScript.SnapAndAlignPartTo(nextSnapPoint);

        if (levelPartScript.IntersectionDetected())
        {
            InitializieGeneration();
            return;
        }


        nextSnapPoint = levelPartScript.GetExitPoint();
        enemyList.AddRange(levelPartScript.MyEnemies());
    }
    private Transform ChooseRandomPart()
    {

        int randomIndex = Random.Range(0, currentLevelParts.Count);

        Transform choosenPart = currentLevelParts[randomIndex];

        currentLevelParts.RemoveAt(randomIndex);

        return choosenPart;
    }
    public Enemy GetRandomEnemy()
    {
        int randomIndex = Random.Range(0, enemyList.Count);
        return enemyList[randomIndex];
    }
    public List<Enemy> GetEnemyList() => enemyList;











}
