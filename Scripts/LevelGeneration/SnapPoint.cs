using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SnapPointType { Enter, Exit}
public class SnapPoint : MonoBehaviour
{
    public SnapPointType snapPointType;


    private void Start()
    {
        BoxCollider boxColld = GetComponent<BoxCollider>();
        MeshRenderer mr = GetComponent<MeshRenderer>();

        if(boxColld != null )
            boxColld.enabled = false;

        if(mr != null )
            mr.enabled = false;

        
    }
    private void OnValidate()
    {
        gameObject.name = "Snap Point - " + snapPointType.ToString();
    }
}
