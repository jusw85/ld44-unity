using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour, IHasHP
{
    #region PUBLIC_VARIABLES

    [Header("Data")]
    public Spell[] spells;
    public int hp;
    public int initialSlaves;
    public GameObject slavePrefab;
    public GameObject spawnArea;

    [Header("Control")]
    public SkillBarController skillBar;
    public GameObject fireball;
    public EnemyController enemy;
    public Slider chargeBar;

    #endregion

    #region PRIVATE_VARIABLES

    private ObjectPooler objectPooler;
    private Bounds[] spawnBounds;
    private Queue<GameObject> slaveObjs;
    private int shieldHp;
    private bool _isCasting;
    private Spell currentButtonPressed;
    private bool isInputFrozen;

    #endregion

    #region CACHED_VARIABLES

    private Animator animator;
    private Transform muzzle;
    private GameObject shield;

    #endregion

    #region UNITY_CALLBACKS

    private void Awake()
    {
        animator = GetComponent<Animator>();
        muzzle = transform.GetChild(0);
        shield = transform.GetChild(1).gameObject;

        _isCasting = false;
        slaveObjs = new Queue<GameObject>();

        currentButtonPressed = null;
    }

    private void Update()
    {
        if (isInputFrozen)
        {
            return;
        }

        Spell buttonPress = null;
        foreach (Spell s in spells)
        {
            if (Input.GetKey(s.Key))
            {
                buttonPress = s;
                break;
            }
        }

        if (currentButtonPressed == null && buttonPress != null && isCasting() == false)
        {
            SetCharging(true);
            currentButtonPressed = buttonPress;
        }

        if (currentButtonPressed != null)
        {
            if (currentButtonPressed == buttonPress)
            {
                chargeBar.value += Time.deltaTime / currentButtonPressed.Cooldown;
                chargeBar.value = Mathf.Clamp(chargeBar.value, 0f, 1f);
                if (chargeBar.value >= 1)
                {
                    HandleSkill(currentButtonPressed.Key.ToString(), currentButtonPressed.Cost);
                    chargeBar.value = 0f;
                    currentButtonPressed = null;
                }
            }
            else
            {
                SetCharging(false);
                chargeBar.value = 0f;
                currentButtonPressed = null;
            }
        }
    }

    private void Start()
    {
        objectPooler = ObjectPooler.instance;
        objectPooler.CreatePool(slavePrefab, Mathf.Max(200, initialSlaves));

        BoxCollider2D[] colliders = spawnArea.GetComponentsInChildren<BoxCollider2D>();
        spawnBounds = new Bounds[colliders.Length];
        for (int i = 0; i < spawnBounds.Length; i++)
        {
            spawnBounds[i] = colliders[i].bounds;
        }

        SpawnSlaves(initialSlaves);

        foreach (var skill in spells)
        {
            skillBar.CreateSkill(skill.SpellName, skill.Key.ToString(), skill.Icon);
        }
    }

    #endregion

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
            var obj = objectPooler.GetPoolObject(slavePrefab);
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
        isInputFrozen = isFrozen;
        if (isFrozen)
        {
            chargeBar.value = 0f;
            currentButtonPressed = null;
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