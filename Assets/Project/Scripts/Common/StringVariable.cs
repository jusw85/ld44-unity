using UnityEngine;

[CreateAssetMenu]
public class StringVariable : ScriptableObject
{
#if UNITY_EDITOR
    [Multiline] public string DeveloperDescription = "";
#endif
    [SerializeField] private string defaultValue;
    private string value;

    public string DefaultValue => defaultValue;

    public string Value
    {
        get { return value; }
        set { this.value = value; }
    }

    private void OnEnable()
    {
        value = defaultValue;
    }
}