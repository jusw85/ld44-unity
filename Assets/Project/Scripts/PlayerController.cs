using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject slave;
    public int numSlaves;

    private void Start()
    {
        var objectPooler = ObjectPooler.instance;
        objectPooler.CreatePool(slave, Mathf.Max(50, numSlaves));
        for (int i = 0; i < numSlaves; i++)
        {
            var obj = objectPooler.GetPoolObject(slave);
            obj.SetActive(true);
            obj.transform.position = transform.position;
            obj.transform.rotation = Quaternion.identity;
        }
    }
}