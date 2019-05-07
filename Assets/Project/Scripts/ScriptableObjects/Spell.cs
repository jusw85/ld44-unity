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
        keyCode = (KeyCode) Enum.Parse(typeof(KeyCode), key);
    }
}