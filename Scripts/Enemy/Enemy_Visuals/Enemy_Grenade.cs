using System.Collections.Generic;
using UnityEngine;

public class Enemy_Grenade : MonoBehaviour
{
    [SerializeField] private GameObject explosionFX;
    [SerializeField] private float impactRadius;
    [SerializeField] private float upwardsMultiplier = 1f;
    [SerializeField] private LayerMask allyLayerMask;
    private Rigidbody rb;
    private float timer;
    private float impactPower;

    private int grenadeDamage;

    private bool canExplode = true;
    private void Awake() => rb = GetComponent<Rigidbody>();


    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0 && canExplode)
            Explode();

    }

    private void Explode()
    {
        canExplode = false;

        PlayExplosionFX();

        HashSet<GameObject> UniqueEntities = new HashSet<GameObject>();

        Collider[] colliders = Physics.OverlapSphere(transform.position, impactRadius);

        foreach (Collider hit in colliders)
        {
            IDamageble damagable = hit.GetComponent<IDamageble>();

            if (damagable != null)
            {

                if (IsTargetValid(hit) == false)
                    continue;

                GameObject rootEntity = hit.transform.root.gameObject; // root parent objeye gitmek için.
                if (UniqueEntities.Add(rootEntity) == false)
                    continue;


                damagable.TakeDamage(grenadeDamage);
            }




            ApplyPhysicalForceTo(hit);
        }
        ObjectPool.instance.ReturnObject(gameObject);
    }

    private void ApplyPhysicalForceTo(Collider hit)
    {
        Rigidbody rb = hit.GetComponent<Rigidbody>();

        if (rb != null)
            rb.AddExplosionForce(impactPower, transform.position, impactRadius, upwardsMultiplier, ForceMode.Impulse);
    }



    private void PlayExplosionFX()
    {
        GameObject newFX = ObjectPool.instance.GetObject(explosionFX, transform);
        ObjectPool.instance.ReturnObject(newFX, 1);
        ObjectPool.instance.ReturnObject(gameObject);
    }

    public void SetupGrenade(LayerMask allyLayermask, Vector3 target, float timeToTarget, float countdown, float impactPower,int grenadeDamage)
    {
        this.grenadeDamage = grenadeDamage;
        canExplode = true;
        this.allyLayerMask = allyLayermask;
        rb.velocity = CalculateLaunchVelocity(target, timeToTarget);
        timer = countdown + timeToTarget;
        this.impactPower = impactPower;
    }

    private bool IsTargetValid(Collider collider)
    {
        //friendly fire aktifse bütün targetlar valid

        if (GameManager.instance.friendlyFire)
            return true;

        // collider allylayerdaysa valid değil

        if ((allyLayerMask.value & (1 << collider.gameObject.layer)) > 0)
            return false;

        return true;
    }

    private Vector3 CalculateLaunchVelocity(Vector3 target, float timeToTarget)
    {
        Vector3 direction = target - transform.position;
        Vector3 directionXZ = new Vector3(direction.x, 0, direction.z);

        Vector3 velocityXZ = directionXZ / timeToTarget;  // atılan yönü zaman bölünce hızı buluyoruz

        float velocityY =
            (direction.y - (Physics.gravity.y * Mathf.Pow(timeToTarget, 2)) / 2) / timeToTarget;

        Vector3 launchVelocity = velocityXZ + Vector3.up * velocityY;

        return launchVelocity;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, impactRadius);
    }
}
