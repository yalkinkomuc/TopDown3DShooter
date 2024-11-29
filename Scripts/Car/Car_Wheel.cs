using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AxelType { Front, Rear }

[RequireComponent(typeof(WheelCollider))]
public class Car_Wheel : MonoBehaviour
{

    public AxelType axelType;
    public WheelCollider wcd {  get; private set; }
    public TrailRenderer trailRenderer {  get; private set; }   
    public GameObject model;

    private float defaultSideStifness;

    private void Awake()
    {
        wcd = GetComponent<WheelCollider>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();
        trailRenderer.emitting = false;
        if(model == null )
        model = GetComponentInChildren<MeshRenderer>().gameObject;

      
    }

    public void SetDefaultStiffness(float newValue)
    {
        defaultSideStifness = newValue;
        RestoreDefaultStifness();
    }
    public void RestoreDefaultStifness()
    {
        WheelFrictionCurve sidewayFriction = wcd.sidewaysFriction;
        sidewayFriction.stiffness = defaultSideStifness;
        wcd.sidewaysFriction = sidewayFriction;
    }
}
