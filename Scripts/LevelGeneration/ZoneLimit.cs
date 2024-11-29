using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneLimit : MonoBehaviour
{
    private ParticleSystem[] lines;
    private BoxCollider zoneColliders;

    private void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
        zoneColliders = GetComponent<BoxCollider>();
        lines = GetComponentsInChildren<ParticleSystem>();
        ActivateWall(false);
    }

    private void ActivateWall(bool activate)
    {
        foreach (var line in lines)
        {
            if (activate)
            {
                line.Play();
            }
            else
            {
                line.Stop();
            }
            
        }

        zoneColliders.isTrigger = !activate;

    }

    IEnumerator WallActivationCo()
    {
        ActivateWall(true);

        yield return new WaitForSeconds(1);

        ActivateWall(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(WallActivationCo());
        Debug.Log("My Sensors are going crazy, i think its's a dangerous area.");
    }

}
