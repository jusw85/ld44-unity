using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Spell")]
public class Spell : ScriptableObject
{
    public String spellName;
    public String key;
    public Sprite icon;
    public float cooldown;
    public int cost;

    [NonSerialized]
    public KeyCode keyCode;

    private void Awake()
    {
        Debug.Log("AWAKE!");
        keyCode = (KeyCode) Enum.Parse(typeof(KeyCode), key);
    }

    private void OnEnable()
    {
        Debug.Log("ENABLE!");
    }
    private void OnDisable()
    {
        Debug.Log("DISABLE!");
    }
    private void OnDestroy()
    {
        Debug.Log("DESTROY!");
    }

    private void OnValidate()
    {
        Debug.Log("VALIDATE!");
    }
}