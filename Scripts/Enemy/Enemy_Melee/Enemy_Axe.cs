using UnityEngine;

public class Enemy_Axe : MonoBehaviour
{
    [SerializeField] private GameObject impactFX;
    public Rigidbody rb;
    private Transform player;
    public Transform axeVisual;

    public float rotationSpeed;
    private float flySpeed;
    private Vector3 direction;

    private float timer = 1;

    private int damage;


    private void Update()
    {

        axeVisual.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        timer -= Time.deltaTime;

        if (timer > 0)
            direction = player.position + Vector3.up - transform.position;


        transform.forward = rb.velocity;
    }

    private void FixedUpdate()
    {
        rb.velocity = direction.normalized * flySpeed;
        
    }

    public void SetupAxe(float flySpeed, Transform player, float timer,int damage)
    {
        rotationSpeed = 1600;

        this.damage = damage;
        this.flySpeed = flySpeed;
        this.player = player;
        this.timer = timer;
    }


    private void OnCollisionEnter(Collision collision)
    {
        IDamageble damagable = collision.gameObject.GetComponent<IDamageble>();

       


        damagable?.TakeDamage(damage);

        GameObject newFX = ObjectPool.instance.GetObject(impactFX, transform);


        ObjectPool.instance.ReturnObject(gameObject);
        ObjectPool.instance.ReturnObject(newFX, 1f);
    }
}
