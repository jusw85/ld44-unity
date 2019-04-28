using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler instance;

    private Dictionary<int, Queue<GameObject>> pools;

    private List<GameObject> pooledObjects;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        pools = new Dictionary<int, Queue<GameObject>>();
    }

    public bool CreatePool(GameObject obj, int size)
    {
        int id = obj.GetInstanceID();
        if (pools.ContainsKey(id))
        {
            return false;
        }

        Queue<GameObject> queue = new Queue<GameObject>();
        for (int i = 0; i < size; i++)
        {
            var go = Instantiate(obj);
            go.SetActive(false);
            queue.Enqueue(go);
        }

        pools.Add(id, queue);
        return true;
    }

    public GameObject GetPoolObject(GameObject obj)
    {
        int id = obj.GetInstanceID();
        Queue<GameObject> queue;
        pools.TryGetValue(id, out queue);
        if (queue == null)
        {
            return null;
        }

        var res = queue.Dequeue();
        queue.Enqueue(res);
        return res;
    }
}