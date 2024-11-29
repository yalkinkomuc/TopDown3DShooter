using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;



    [SerializeField] private int poolSize = 10;

    [SerializeField] private GameObject weaponPickup;
    [SerializeField] private GameObject ammoPickup;

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        InitializeNewPool(weaponPickup);
        InitializeNewPool(ammoPickup);
    }

    public GameObject GetObject(GameObject prefab, Transform target)
    {
        if (poolDictionary.ContainsKey(prefab) == false)
        {
            InitializeNewPool(prefab);
        }

        if (poolDictionary[prefab].Count == 0)
            CreateNewObject(prefab);

        GameObject ObjectsToGet = poolDictionary[prefab].Dequeue();

        ObjectsToGet.transform.position = target.position;

        ObjectsToGet.SetActive(true);
        ObjectsToGet.transform.parent = null;

        return ObjectsToGet;
    }

    public void ReturnObject(GameObject objectToReturn, float delay = .001f) => StartCoroutine(DelayReturn(delay, objectToReturn));

    private IEnumerator DelayReturn(float delay, GameObject objectToReturn)
    {
        yield return new WaitForSeconds(delay);

        ReturnToPool(objectToReturn);
    }
    private void ReturnToPool(GameObject ObjectToReturn)
    {
        

        GameObject originalPrefab = ObjectToReturn.GetComponent<PooledObjects>().originalPrefab;

        ObjectToReturn.SetActive(false);


        ObjectToReturn.transform.parent = transform;

        poolDictionary[originalPrefab].Enqueue(ObjectToReturn);
    }
    public void InitializeNewPool(GameObject prefab)
    {

        poolDictionary[prefab] = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject(prefab);
        }
    }

    private void CreateNewObject(GameObject prefab)
    {
        GameObject newObject = Instantiate(prefab, transform); // poolsize sayısı kaçsa o sayıya kadar mermi üretir ve awake içinde stoklar.
        newObject.AddComponent<PooledObjects>().originalPrefab = prefab;
        newObject.SetActive(false);

        poolDictionary[prefab].Enqueue(newObject);
    }
}
