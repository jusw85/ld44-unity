using UnityEngine;

[CreateAssetMenu]
public class FloatVariable : ScriptableObject
{
#if UNITY_EDITOR
    [Multiline] public string DeveloperDescription = "";
#endif
    [SerializeField] private float defaultValue;
    private float value;

    public float DefaultValue => defaultValue;

    public float Value
    {
        get { return value; }
        set { this.value = value; }
    }

    private void OnEnable()
    {
        value = defaultValue;
    }
}