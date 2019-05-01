using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour, IHasHP
{
    public int hp;
    public GameObject slave;
    public int initialSlaves;
    public GameObject spawnArea;
    public SkillBarController skillBar;
    public GameObject fireball;
    public EnemyController enemy;

    private Animator animator;
    private ObjectPooler objectPooler;
    private Bounds[] spawnBounds;
    private Queue<GameObject> slaveObjs;

    private Transform muzzle;
    private GameObject shield;

    private int shieldHp;
    private bool _isCasting;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        _isCasting = false;
        muzzle = transform.Find("Muzzle");
        shield = transform.Find("Shield").gameObject;
        slaveObjs = new Queue<GameObject>();
    }

    private void Start()
    {
        objectPooler = ObjectPooler.instance;
        objectPooler.CreatePool(slave, Mathf.Max(200, initialSlaves));

        BoxCollider2D[] colliders = spawnArea.GetComponentsInChildren<BoxCollider2D>();
        spawnBounds = new Bounds[colliders.Length];
        for (int i = 0; i < spawnBounds.Length; i++)
        {
            spawnBounds[i] = colliders[i].bounds;
        }

        SpawnSlaves(initialSlaves);
    }

    public void HandleSkill(String key, int cost)
    {
        SacSlaves(cost);
        switch (key)
        {
            case "Q":
                SpawnFireball(2, () => { enemy.TakeDamage(3); });
                break;
            case "W":
                SpawnFireball(1, () =>
                {
                    enemy.TakeDamage(1);
                    enemy.Freeze();
                });
                break;
            case "E":
                SpawnShield();
                break;
            case "R":
                SpawnFireball(0, () => { enemy.TakeDamage(7); });
                break;
            default:
                break;
        }

        animator.SetTrigger("cast");
        _isCasting = true;
    }

    private void SpawnShield()
    {
        shieldHp = 2;
        shield.SetActive(true);
        shield.GetComponent<Animator>().SetTrigger("activate");
    }

    public bool isShielded()
    {
        return shieldHp > 0;
    }

    private void SpawnFireball(int fireballType, Action postComplete = null)
    {
        var obj = Instantiate(fireball, muzzle.position, Quaternion.identity);
        obj.SetActive(true);
        obj.GetComponent<SpriteRenderer>().flipX = false;
        var anim = obj.GetComponent<Animator>();
        anim.SetLayerWeight(0, 0);
        anim.SetLayerWeight(fireballType, 1);

        DOTween.To(() => obj.transform.position,
                x => obj.transform.position = x,
                enemy.transform.position, 1f)
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
            obj.GetComponent<SpriteRenderer>().flipX = false;
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

            anim.SetLayerWeight(Random.Range(0, 3), 1f);
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
        if (shieldHp > 0)
        {
            shieldHp--;
            if (shieldHp <= 0)
            {
                shield.SetActive(false);
            }

            return;
        }

        hp -= dmg;
        if (hp <= 0)
        {
            SceneManager.LoadScene(sceneName: "LoseScreen");
        }
    }

    public int GetHP()
    {
        return hp;
    }

    public void FreezeInput(bool isFrozen)
    {
        skillBar.FreezeInput(isFrozen);
        if (isFrozen)
        {
            animator.SetTrigger("doIdle");
        }
    }

    public Vector3 GetFireTarget()
    {
        if (isShielded())
        {
            return shield.transform.position;
        }

        return transform.position;
    }

    public void SetCharging(bool isCharging)
    {
        animator.SetTrigger(isCharging ? "charging" : "doIdle");
    }

    public bool isCasting()
    {
        return _isCasting;
    }

    public void SetIsCasting(bool isCasting)
    {
        _isCasting = isCasting;
    }
}