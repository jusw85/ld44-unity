using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject slave;
    public int numSlaves;
    public GameObject spawnArea;

    private ObjectPooler objectPooler;

    private void Start()
    {
        objectPooler = ObjectPooler.instance;
        objectPooler.CreatePool(slave, Mathf.Max(50, numSlaves));

        BoxCollider2D[] colliders = spawnArea.GetComponentsInChildren<BoxCollider2D>();

        for (int i = 0; i < numSlaves; i++)
        {
            var obj = objectPooler.GetPoolObject(slave);
            obj.SetActive(true);
            var bounds = colliders[Random.Range(0, colliders.Length)].bounds;
            obj.transform.position = RandomPointInBounds(bounds);
            obj.transform.rotation = Quaternion.identity;
        }
    }

    private void SpawnSlave()
    {
    }

    private static Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}