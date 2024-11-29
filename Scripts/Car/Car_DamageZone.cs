using UnityEngine;

public class Car_DamageZone : MonoBehaviour
{
    private Car_Controller carController;
    [SerializeField] private float minSpeedToDamage = 1.5f;
    [SerializeField] private float impactForce = 150;
    [SerializeField] private float upWardsMultiplier = 3;


    [SerializeField] private int carDamage;

    private void Awake()
    {
        carController = GetComponentInParent<Car_Controller>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(carController.rb.velocity.magnitude < minSpeedToDamage)
            return;

        IDamageble damagable = other.GetComponent<IDamageble>();
        if (damagable == null)
            return;

        damagable.TakeDamage(carDamage);

        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (rb != null)
            ApplyForce(rb);
    }

    private void ApplyForce(Rigidbody rb)
    {
       
        rb.isKinematic = false;
        rb.AddExplosionForce(impactForce,transform.position,3,upWardsMultiplier,ForceMode.Impulse);
    }
}
