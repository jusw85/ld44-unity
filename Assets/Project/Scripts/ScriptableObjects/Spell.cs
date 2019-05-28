using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Spell")]
public class Spell : ScriptableObject
{
    [SerializeField] private String spellName;
    [SerializeField] private KeyCode key;
    [SerializeField] private Sprite icon;
    [SerializeField] private float cooldown;
    [SerializeField] private int cost;

    public string SpellName => spellName;

    public KeyCode Key => key;

    public Sprite Icon => icon;

    public float Cooldown => cooldown;

    public int Cost => cost;
}