using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{

    private Transform playerTransform;

    [SerializeField] private GameObject coverPointsPrefab;
    [SerializeField] private List<CoverPoint> coverPoints = new List<CoverPoint>();
    [SerializeField] private float xOffset = 1;
    [SerializeField] private float yOffset = .2f;
    [SerializeField] private float zOffset = 1;

    private void Start()
    {
        GenerateCoverPoints();
        playerTransform = FindObjectOfType<Player>().transform ;
        
    }


    private void GenerateCoverPoints()
    {
        Vector3[] localCoverPoints =
        {
            new Vector3 (0,yOffset,zOffset), //Ön
            new Vector3 (0,yOffset,-zOffset), // Arka
            new Vector3 (xOffset,yOffset,0), //Sağ
            new Vector3 (-xOffset,yOffset,0) // Sol



        };

        foreach (Vector3 localPoint in localCoverPoints)
        {
            Vector3 worldPoint = transform.TransformPoint (localPoint);  //local spaceten world space e çeviriyoruz.
            CoverPoint coverpoint = Instantiate(coverPointsPrefab,worldPoint,Quaternion.identity,transform).GetComponent<CoverPoint>();
            coverPoints.Add (coverpoint);
        }

        DisableCoverPointsMeshRenderer();

    }

    private void DisableCoverPointsMeshRenderer()
    {
        foreach (MeshRenderer mr in coverPointsPrefab.GetComponentsInChildren<MeshRenderer>()) { mr.enabled = false; }

    }

    public List<CoverPoint> GetValidCoverPoints(Transform enemy)
    {
        List<CoverPoint> validCoverPoints = new List<CoverPoint>();  //validcoverpoints adında içinde coverpoints olan liste oluşturduk.

        foreach (CoverPoint coverPoint in coverPoints) // if içerisinde koşulları sağlayan her elemani validcoverpointse ekledik.
        {
            if(IsValidCoverPoint(coverPoint,enemy))
            validCoverPoints.Add(coverPoint);
        }

        return validCoverPoints;
    }

    private bool IsValidCoverPoint(CoverPoint coverPoint,Transform enemy)
    {
        if(coverPoint.occupied)   //cover doluysa
            return false;

        if(IsFurthestFromPlayer(coverPoint)==false) //eğer en uzak cover değilse false döndür.
            return false;

        if(isCoverBehindPlayer(coverPoint,enemy))   //cover oyuncunun arkasındaysa
            return false;

        if(isCoverCloseToPlayer(coverPoint)) // cover oyuncuya yakınsa false
            return false;

        if(isCoverCloseToLastCover(coverPoint,enemy)) // cover bir öncekine yakınsa 
            return false;


        return true;
    }

    private bool IsFurthestFromPlayer(CoverPoint coverPoint)
    {
        CoverPoint furthestPoint = null;  //Değerleri Resetliyoruz
        float furthestDistance = 0;

        foreach (CoverPoint point in coverPoints)
        {
            float distance = Vector3.Distance(point.transform.position,playerTransform.transform.position);
            if(distance > furthestDistance)
            {
                furthestDistance = distance;
                furthestPoint = point;  
            }
        }
        return furthestPoint == coverPoint;
    }

    private bool isCoverBehindPlayer(CoverPoint coverPoint, Transform enemy)
    {
        float distanceToPlayer = Vector3.Distance(coverPoint.transform.position,playerTransform.position); // cover ile player arası mesafe
        float distanceToEnemy = Vector3.Distance(coverPoint.transform.position,enemy.position); // cover ile enemy arası mesafe

        return distanceToPlayer < distanceToEnemy;   //eğer enemy ile mesafe player ile mesafeden büyükse true
    }

    private bool isCoverCloseToPlayer (CoverPoint coverPoint)
    {
       return Vector3.Distance(coverPoint.transform.position,playerTransform.position) < 2f;
    }

    private bool isCoverCloseToLastCover (CoverPoint coverPoint,Transform enemy)
    {
        CoverPoint lastCover = enemy.GetComponent<Enemy_Range>().currentCover;

        return lastCover != null && Vector3.Distance(coverPoint.transform.position, lastCover.transform.position) < 3;
        // iki cover arası mesafe 3 ten kücükse ve lastcover boş değilse true
    }
}
