using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelParts : MonoBehaviour
{
    [SerializeField] private LayerMask intersectionLayer;
    [SerializeField] private Collider[] intersectionColliders;
    [SerializeField] private Transform intersectionCheckParent;


    [ContextMenu("Set static to environment layer")]
    private void AdjustLayerForStaticObjects()
    {
        foreach (Transform childTransform in transform.GetComponentsInChildren<Transform>(true))
        {
            if (childTransform.gameObject.isStatic)
            {
                childTransform.gameObject.layer = LayerMask.NameToLayer("Environment");
            }
        }
    }
        
        
    private void Start()
    {
        if(intersectionColliders.Length <= 0)
        {
            intersectionColliders = intersectionCheckParent.GetComponentsInChildren<Collider>();
        }


    }
    public bool IntersectionDetected()
    {
        Physics.SyncTransforms(); // physic checki doğru konumdan yapabilmek icin

        foreach (var collider in intersectionColliders) 
        {
            Collider[] hitColliders = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, Quaternion.identity, intersectionLayer); // ic ice giren colliderları buluyoruz


            foreach (var hit in hitColliders)
            {
                IntersectionCheck intersectionCheck = hit.GetComponentInParent<IntersectionCheck>(); // intersection checki arıyoruz 

                if (intersectionCheck != null&&intersectionCheckParent != intersectionCheck.transform)
                    return true;
            }
        }

       return false;
        

    }

    

    private void AlignTo(SnapPoint ownSnapPoint,SnapPoint targetSnapPoint)
    {

       

        var rotationOffset = ownSnapPoint.transform.rotation.eulerAngles.y - transform.rotation.eulerAngles.y;

        transform.rotation = targetSnapPoint.transform.rotation;

        transform.Rotate(0, 180, 0);
        transform.Rotate(0,-rotationOffset, 0);
    }





    private void SnapTo(SnapPoint ownSnapPoint , SnapPoint targetSnapPoint)
    {
        var offSet = transform.position - ownSnapPoint.transform.position;

        var newPosition = targetSnapPoint.transform.position + offSet;  

        transform.position = newPosition;
    }

    public SnapPoint GetEntrancePoint() => GetSnapPointsOfType(SnapPointType.Enter);
    public SnapPoint GetExitPoint() => GetSnapPointsOfType(SnapPointType.Exit);


   private SnapPoint GetSnapPointsOfType(SnapPointType pointType)
    {
        SnapPoint[] snapPoints = GetComponentsInChildren<SnapPoint>();
        List<SnapPoint> filteredSnapPoints = new List<SnapPoint>();


        // belirli tipteki bütün snap pointleri al 

        foreach (SnapPoint snapPointss in snapPoints)
        {
            if(snapPointss.snapPointType == pointType)
                filteredSnapPoints.Add(snapPointss);
        }

        // eğer eşleşen snap point varsa rastgele birini sec 

        if (filteredSnapPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, filteredSnapPoints.Count);
            return filteredSnapPoints[randomIndex];
        }

        // eşleşen snap point yoksa null

        return null;
    }

    public void SnapAndAlignPartTo(SnapPoint targetSnapPoint)
    {
        SnapPoint entrancePoint = GetEntrancePoint();

        AlignTo(entrancePoint, targetSnapPoint);
        SnapTo(entrancePoint, targetSnapPoint);
    }

    public Enemy[] MyEnemies() => GetComponentsInChildren<Enemy>(true); 

}
