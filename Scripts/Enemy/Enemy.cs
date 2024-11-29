using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public enum EnemyType { Melee, Range, Boss,Random }

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public EnemyType enemyType;
    public LayerMask whatisPlayer;
    public LayerMask whatisAlly;

    [Space]


    [Header("Idle Info")]
    public float idleTime;
    public float aggresionRange;

    [Space]


    [Header("Move Info")]
    public float walkSpeed = 1.5f;
    public float runSpeed = 3;
    private bool manuelMovement;
    private bool manuelRotation;
    public float turnSpeed;

    [Space]


    public NavMeshAgent agent;
    public Animator anim;



    public bool inBattleMode { get; private set; }
    protected bool isMeleeAttackReady;

    [Space]


    [SerializeField] private Transform[] patrolPoints;
    private Vector3[] patrolPointsPosition;
    private int currentPatrolIndex;




    public EnemyVisuals visuals { get; private set; }
    public Transform player { get; private set; }

    public EnemyStateMachine stateMachine { get; private set; }
    public EnemyHealth health { get; private set; }
    public Ragdoll ragdoll { get; private set; }
    public EnemyDropController dropController { get; private set; }
    public AudioManager audioManager { get; private set; }

    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();
        ragdoll = GetComponent<Ragdoll>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        player = GameObject.Find("Player").GetComponent<Transform>();
        visuals = GetComponent<EnemyVisuals>();
        health = GetComponent<EnemyHealth>();
        dropController = GetComponent<EnemyDropController>();
    }

    protected virtual void Start()
    {
        InitalizePatrolPoints();
        audioManager = AudioManager.instance;
    }

    protected virtual void InitializeSpeciality()
    {

    }

    protected bool ShouldEnterBattleMode()
    {

        if (IsPlayerOnAggresionRange() && !inBattleMode)
        {
            EnterBattleMode();
            return true;
        }

        return false;
    }

    protected virtual void Update()
    {
        if (ShouldEnterBattleMode())
            EnterBattleMode();
    }

    public virtual void EnterBattleMode()
    {
        inBattleMode = true;
        //AudioManager.instance.PlayBGM(0);
    }

    public virtual void BulletImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        if (health.ShouldDie())
            StartCoroutine(DeathImpactCoroutine(force, hitPoint, rb));
    }

    private IEnumerator DeathImpactCoroutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        yield return new WaitForSeconds(.1f);

        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }
    public virtual void GetHit(int damage)
    {
        EnterBattleMode();
        health.ReduceHealth(damage);

        if (health.ShouldDie())
            Die();


    }

    public virtual void Die()
    {
        dropController.DropItems();

        anim.enabled = false;
        agent.isStopped = true;
        agent.enabled = false ;

        ragdoll.RagdollActive(true);

        MissionObjectHuntTarget huntTarget = GetComponent<MissionObjectHuntTarget>();
        huntTarget?.InvokeOnTargetKilled();
    }

    public virtual void MeleeAttackCheck(Transform[] damagePoints, float attackCheckRadius, GameObject fx, int damage)
    {
        if (isMeleeAttackReady == false)
            return;

        foreach (Transform attackpoints in damagePoints)
        {
            Collider[] detectedHits = Physics.OverlapSphere(attackpoints.position, attackCheckRadius, whatisPlayer); // colliderin playera değip değmediğini kontrol ediyoruz.

            for (int i = 0; i < detectedHits.Length; i++)
            {
                IDamageble damagable = detectedHits[i].GetComponent<IDamageble>();  // değen colliderlar için idamagable scripti var mı diye bakıyoruz

                if (damagable != null)
                {

                    damagable.TakeDamage(damage); // varsa take damage çağırıyoruz.
                    isMeleeAttackReady = false;
                    GameObject newAttackFX = ObjectPool.instance.GetObject(fx, attackpoints);
                    ObjectPool.instance.ReturnObject(newAttackFX, 1);
                    return;
                }

            }


        }

    }

    public void EnableMeleeAttackCheck(bool enable) => isMeleeAttackReady = enable;
    public void FaceTarget(Vector3 target, float turnSpeed = 0)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position); // hedef yön

        Vector3 currentEulerAngels = transform.rotation.eulerAngles; //güncel yön

        if (turnSpeed == 0)
            turnSpeed = this.turnSpeed;

        float yRotation = Mathf.LerpAngle(currentEulerAngels.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime); // sadece y ekseninde döndürmek istediğimiz için  y yi lerpledik

        transform.rotation = Quaternion.Euler(currentEulerAngels.x, yRotation, currentEulerAngels.z);
    }
    public virtual void AbilityTrigger()
    {
        stateMachine.currentState.AbilityTrigger();
    }

    public virtual void MakeEnemyVIP()
    {
        int additionalHealth = Mathf.RoundToInt(health.currentHealth * 1.5f);

        health.currentHealth += additionalHealth;

        transform.localScale = transform.localScale * 1.25f;
    }

    #region Anim Events

    public void ActivateManuelMovement(bool manuelMovement) => this.manuelMovement = manuelMovement;
    public bool ManuelMovementActive() => manuelMovement;
    public void ActivateManuelRotation(bool manuelRotation) => this.manuelRotation = manuelRotation;
    public bool ManuelRotationActive() => manuelRotation;
    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();

    #endregion

    #region Patrol Region

    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPointsPosition[currentPatrolIndex];

        currentPatrolIndex++;

        if (currentPatrolIndex >= patrolPoints.Length)
            currentPatrolIndex = 0;


        return destination;
    }
    private void InitalizePatrolPoints()
    {
        patrolPointsPosition = new Vector3[patrolPoints.Length];

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            patrolPointsPosition[i] = patrolPoints[i].position;
            patrolPoints[i].gameObject.SetActive(false);
        }
    }

    #endregion

    public bool IsPlayerOnAggresionRange() => Vector3.Distance(transform.position, player.position) < aggresionRange;



    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggresionRange);

    }

}
