using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Position")]
public class SoPosition : ScriptableObject
{
#if UNITY_EDITOR
    [SerializeField] [Multiline] private string developerDescription;
#endif
    [SerializeField] private Vector3 defaultPosition;
    [SerializeField] private Quaternion defaultRotation;
    private Vector3 position;
    private Quaternion rotation;

    public Vector3 DefaultPosition => defaultPosition;
    public Quaternion DefaultRotation => defaultRotation;

    public Vector3 Position
    {
        get { return position; }
        set { position = value; }
    }

    public Quaternion Rotation
    {
        get { return rotation; }
        set { rotation = value; }
    }

    private void OnEnable()
    {
        position = defaultPosition;
        rotation = defaultRotation;
    }


    public override string ToString()
    {
        return Position.ToString();
    }
    
}