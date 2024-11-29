using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Health_Controller : MonoBehaviour, IDamageble
{
    Car_Controller carController;

    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    public bool carBroken;

    [Header("ExplosionInfo")]
    [SerializeField] private int explosionDamage = 350;
    [SerializeField] private ParticleSystem fireFX;
    [SerializeField] private ParticleSystem explosionFX;
    [Space]
    [SerializeField] private float explosionRadius = 3;
    [SerializeField] private float explosionDelay = 3;
    [SerializeField] private float explosionForce = 7;
    [SerializeField] private float explosionUpwardsMultiplier = 2;
    [SerializeField] private Transform explosionPoint;

    private void Start()
    {

        carController = GetComponent<Car_Controller>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (fireFX.gameObject.activeSelf)
            fireFX.transform.rotation = Quaternion.identity;
    }
    public void ReduceHealth(int damage)
    {
        if (carBroken)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
            BrakeTheCar();

    }

    public void UpdateCarHealthUI()
    {
        UI.instance.inGameUI.UpdateCarHealthUI(currentHealth, maxHealth);
    }
    public void BrakeTheCar()
    {
        carBroken = true;
        carController.DestroyCar();

        fireFX.gameObject.SetActive(true);
        StartCoroutine(ExplosionCO(explosionDelay));
    }
    public void TakeDamage(int damage)
    {

        ReduceHealth(damage);
        UpdateCarHealthUI();
    }

    private IEnumerator ExplosionCO(float delay)
    {
        yield return new WaitForSeconds(delay);



        explosionFX.gameObject.SetActive(true);
        carController.rb.AddExplosionForce(explosionForce, explosionPoint.position,
        explosionRadius, explosionUpwardsMultiplier, ForceMode.Impulse);

        Explode();

    }

    private void Explode()
    {
        HashSet<GameObject> uniqueEntities = new HashSet<GameObject>();

        Collider[] colliders = Physics.OverlapSphere(explosionPoint.position, explosionRadius);

        foreach (Collider hit in colliders)
        {
            IDamageble damagable = hit.GetComponent<IDamageble>();

            if (damagable != null)
            {
                GameObject rootEntity = hit.transform.root.gameObject;

                if (uniqueEntities.Add(rootEntity) == false)
                    continue;

                damagable.TakeDamage(explosionDamage);



                hit.GetComponentInChildren<Rigidbody>().AddExplosionForce(explosionForce, explosionPoint.position, explosionRadius, explosionUpwardsMultiplier, ForceMode.VelocityChange);

            }


        }
    }
}
