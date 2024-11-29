using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float impactForce;
    private int bulletDamage;

    private Rigidbody rb;
    private BoxCollider boxCollider;
    private TrailRenderer trailRenderer;
    private MeshRenderer meshRenderer;

    private LayerMask allyLayerMask;

    [SerializeField] private GameObject bulletImpactFX;
    private Vector3 startPosition;
    private float flyDistance;
    private bool bulletDisabled;


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        trailRenderer = GetComponent<TrailRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetupBullet(LayerMask allyLayerMask,int bulletDamage, float flyDistance = 100, float impactForce = 100)
    {

        this.allyLayerMask = allyLayerMask;
        this.impactForce = impactForce;
        this.bulletDamage = bulletDamage;

        bulletDisabled = false;
        boxCollider.enabled = true;
        meshRenderer.enabled = true;
        trailRenderer.Clear();
        trailRenderer.time = .25f;
        startPosition = transform.position;
        this.flyDistance = flyDistance + .5f;
    }

    protected void Update()
    {
        UpdateTrailVisuals();
        CheckIfShouldBeDisabled();
        CheckIfShouldReturnToPool();

        //Debug.Log(transform.position);
    }

    protected void CheckIfShouldReturnToPool()
    {
        if (trailRenderer.time < 0)
            ReturnBulletToPool();
    }

    protected void CheckIfShouldBeDisabled()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance && !bulletDisabled) //eğer merminin ateşlendiği yer ile olduğu konum fly distanceden büyükse returnBullet
        {
            boxCollider.enabled = false;
            meshRenderer.enabled = false;
            bulletDisabled = true;
        }
    }

    protected void UpdateTrailVisuals()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance - 1.5f) // mermi final konumuna ulaşmadan 1.5 saniye önce 
            trailRenderer.time -= 2 * Time.deltaTime;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {

        //Debug.Log("Collision Detected with: " + collision.gameObject.name);


        if (FriendlyFire() == false)
        {
            if ((allyLayerMask.value & (1 << collision.gameObject.layer)) > 0)
            {

                ReturnBulletToPool(10);
                return;

            }
        }

           


        CreateImpactFX();
        ReturnBulletToPool();

        IDamageble damageble = collision.gameObject.GetComponent<IDamageble>();
        damageble?.TakeDamage(bulletDamage);

       

        ApplyBulletImpactToEnemy(collision);

    }

    private bool FriendlyFire() => GameManager.instance.friendlyFire;
    private void ApplyBulletImpactToEnemy(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            Vector3 force = rb.velocity.normalized * impactForce;
            Rigidbody hitRigidbody = collision.collider.attachedRigidbody;



            enemy.BulletImpact(force, collision.contacts[0].point, hitRigidbody);
        }
    }

    protected void ReturnBulletToPool(float delay = 0) => ObjectPool.instance.ReturnObject(gameObject,delay);




    protected void CreateImpactFX()
    {
        GameObject newFX = Instantiate(bulletImpactFX);
        newFX.transform.position = transform.position;

        Destroy(newFX,1);

        //GameObject newImpactFX = ObjectPool.instance.GetObject(bulletImpactFX, transform);
        //ObjectPool.instance.ReturnObject(newImpactFX, 1);

    }
}
