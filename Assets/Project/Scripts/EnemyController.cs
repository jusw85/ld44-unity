using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour, IHasHP
{
    public int hp;
    public GameObject slave;
    public int initialSlaves;
    public GameObject spawnArea;
    public GameObject fireball;
    public PlayerController player;
    public SpriteRenderer bg;
    public Sprite[] bgs;
    private int currentBg = 0;

    private Animator animator;
    private ObjectPooler objectPooler;
    private Bounds[] spawnBounds;
    private Queue<GameObject> slaveObjs;

    private Transform muzzle;
    private GameObject shield;
    private SpriteRenderer spriteRenderer;
    private bool isDead;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        objectPooler = ObjectPooler.instance;
        objectPooler.CreatePool(slave, Mathf.Max(200, initialSlaves));
        slaveObjs = new Queue<GameObject>();

        muzzle = transform.Find("Muzzle");
        shield = transform.Find("Shield").gameObject;

        BoxCollider2D[] colliders = spawnArea.GetComponentsInChildren<BoxCollider2D>();
        spawnBounds = new Bounds[colliders.Length];
        for (int i = 0; i < spawnBounds.Length; i++)
        {
            spawnBounds[i] = colliders[i].bounds;
        }

        SpawnSlaves(initialSlaves);

        StartCoroutine(Fire());
    }

    private IEnumerator Fire()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            if (isDead) break;
            SpawnFireball(2, () => { player.TakeDamage(1); });
            SacSlaves(2);
            animator.SetTrigger("cast");
        }
    }

//    public void HandleSkill(String key, int cost)
//    {
//        SacSlaves(cost);
//        switch (key)
//        {
//            case "Q":
//                SpawnFireball(0, () => { enemy.TakeDamage(1); });
//                break;
//            case "W":
//                SpawnFireball(1, () => { enemy.TakeDamage(2); });
//                break;
//            case "E":
//                SpawnShield();
//                break;
//            case "R":
//                SpawnFireball(2, () => { enemy.TakeDamage(3); });
//                break;
//            default:
//                break;
//        }
//
//        animator.SetTrigger("cast");
//    }

    private void SpawnShield()
    {
        shield.SetActive(true);
        shield.GetComponent<Animator>().SetTrigger("activate");
    }

    private void SpawnFireball(int fireballType, Action postComplete = null)
    {
        var obj = Instantiate(fireball, muzzle.position, Quaternion.identity);
        obj.SetActive(true);
        obj.GetComponent<SpriteRenderer>().flipX = true;
        var anim = obj.GetComponent<Animator>();
        anim.SetLayerWeight(0, 0);
        anim.SetLayerWeight(fireballType, 1);

        DOTween.To(() => obj.transform.position,
                x => obj.transform.position = x,
                player.transform.position, 1f)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                anim.SetTrigger("boom");
                if (postComplete != null) postComplete();
            });
    }

    public void SpawnSlaves(int numSlaves)
    {
        for (int i = 0; i < numSlaves; i++)
        {
            var obj = objectPooler.GetPoolObject(slave);
            obj.SetActive(true);
            obj.GetComponent<SpriteRenderer>().flipX = true;
            var bounds = spawnBounds[Random.Range(0, spawnBounds.Length)];
            var xy = RandomPointInBounds(bounds);
            xy.z = 0;
            obj.transform.position = xy;
            obj.transform.rotation = Quaternion.identity;

            Animator anim = obj.GetComponent<Animator>();
            for (int j = 0; j < anim.layerCount; j++)
            {
                anim.SetLayerWeight(j, 0f);
            }

            anim.SetLayerWeight(Random.Range(3, anim.layerCount), 1f);
            slaveObjs.Enqueue(obj);
        }
    }

    public void SacSlaves(int numSlaves)
    {
        int n = Math.Min(numSlaves, slaveObjs.Count);
        for (int i = 0; i < n; i++)
        {
            GameObject obj = slaveObjs.Dequeue();
            obj.GetComponent<Animator>().SetTrigger("slaveSac");
        }
    }

    public int GetNumSlaves()
    {
        return slaveObjs.Count;
    }

    private static Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        hp = Mathf.Clamp(hp, 0, hp);
        if (hp <= 0)
        {
            DoMeDeath();
        }
    }

    private void DoMeDeath()
    {
        isDead = true;
        while (slaveObjs.Count > 0)
        {
            GameObject obj = slaveObjs.Dequeue();
            obj.GetComponent<Animator>().SetTrigger("slaveDeath");
        }

        player.FreezeInput(true);
        spriteRenderer.enabled = false;
        StartCoroutine(NextStage());
    }

    private IEnumerator NextStage()
    {
        yield return new WaitForSeconds(3);
        animator.SetLayerWeight(currentBg, 0f);
        currentBg = (currentBg + 1) % bgs.Length;
        animator.SetLayerWeight(currentBg, 1f);
        spriteRenderer.enabled = true;
        bg.sprite = bgs[currentBg];
        player.FreezeInput(false);
        hp = 20;
        SpawnSlaves(20);
        StartCoroutine(Fire());
    }

    public int GetHP()
    {
        return hp;
    }
}