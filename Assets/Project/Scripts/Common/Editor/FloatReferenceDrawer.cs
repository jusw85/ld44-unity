using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FloatReference))]
public class FloatReferenceDrawer : PropertyDrawer
{
    private readonly string[] popupOptions = {"Use Constant", "Use Variable"};
    private GUIStyle popupStyle;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (popupStyle == null)
        {
            popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"))
            {
                imagePosition = ImagePosition.ImageOnly
            };
        }

        SerializedProperty useConstantProp = property.FindPropertyRelative("UseConstant");
        SerializedProperty constantValueProp = property.FindPropertyRelative("ConstantValue");
        SerializedProperty variableProp = property.FindPropertyRelative("Variable");

        label = EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, label);

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        Rect buttonRect = new Rect
        {
            yMin = position.yMin + popupStyle.margin.top,
            xMin = position.xMin + popupStyle.margin.left,
            width = popupStyle.fixedWidth,
            height = popupStyle.fixedHeight
        };
        position.xMin = buttonRect.xMax + popupStyle.margin.right;

        EditorGUI.BeginChangeCheck();
        useConstantProp.boolValue =
            (EditorGUI.Popup(buttonRect, useConstantProp.boolValue ? 0 : 1, popupOptions, popupStyle) == 0);

        EditorGUI.PropertyField(position, useConstantProp.boolValue ? constantValueProp : variableProp, GUIContent.none);

        if (EditorGUI.EndChangeCheck())
            property.serializedObject.ApplyModifiedProperties();

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}