using UnityEngine;

public class EnemyDropController : MonoBehaviour
{
    [SerializeField] private GameObject missionObjectKey;

    public void GiveKey(GameObject newKey) => missionObjectKey = newKey;

    public void DropItems()
    {
        if (missionObjectKey != null)
            CreateItem(missionObjectKey);

    }

    private void CreateItem(GameObject go)
    {
        GameObject newItem = Instantiate(go, transform.position + Vector3.up, Quaternion.identity);
    }
}
