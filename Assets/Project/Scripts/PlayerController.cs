using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    public GameObject slave;
    public int numSlaves;
    public GameObject spawnArea;

    private Animator animator;
    private ObjectPooler objectPooler;
    private Bounds[] spawnBounds;
    private Queue<GameObject> slaveObjs;

    private void Start()
    {
        animator = GetComponent<Animator>();
        objectPooler = ObjectPooler.instance;
        objectPooler.CreatePool(slave, Mathf.Max(200, numSlaves));
        slaveObjs = new Queue<GameObject>();

        BoxCollider2D[] colliders = spawnArea.GetComponentsInChildren<BoxCollider2D>();
        spawnBounds = new Bounds[colliders.Length];
        for (int i = 0; i < spawnBounds.Length; i++)
        {
            spawnBounds[i] = colliders[i].bounds;
        }

        SpawnSlaves(15);
    }

    public void HandleSkill(String key)
    {
        switch (key)
        {
            case "Q":
                SacSlaves(1);
                break;
            case "W":
                SacSlaves(2);
                break;
            case "E":
                SacSlaves(3);
                break;
            case "R":
                SacSlaves(4);
                break;
            default:
                break;
        }

        animator.Play("PlayerCast");
    }

    public void SpawnSlaves(int numSlaves)
    {
        for (int i = 0; i < numSlaves; i++)
        {
            var obj = objectPooler.GetPoolObject(slave);
            obj.SetActive(true);
            var bounds = spawnBounds[Random.Range(0, spawnBounds.Length)];
            var xy = RandomPointInBounds(bounds);
            xy.z = 0;
            obj.transform.position = xy;
            obj.transform.rotation = Quaternion.identity;
            slaveObjs.Enqueue(obj);
        }
    }

    public void SacSlaves(int numSlaves)
    {
        int n = Math.Min(numSlaves, slaveObjs.Count);
        for (int i = 0; i < n; i++)
        {
            GameObject obj = slaveObjs.Dequeue();
            obj.GetComponent<Animator>().Play("SlaveSac");
        }
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