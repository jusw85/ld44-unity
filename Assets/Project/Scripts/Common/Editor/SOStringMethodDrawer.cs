using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SOStringMethod))]
public class SOStringMethodDrawer : PropertyDrawer
{
    private string[] toStringMethods(UnityEngine.Object obj)
    {
//        var mis = textSource.GetType().GetMethods();
//        foreach (var mi in mis)
//        {
//            if (mi.Name.Equals("ToString"))
//            {
//                Debug.Log(mi.IsSpecialName); // false
//                Debug.Log(mi.DeclaringType); // FloatVariable
//                Debug.Log(mi.DeclaringType == textSource.GetType()); // true
//            }
//
//            if (mi.Name.Equals("get_name"))
//            {
//                Debug.Log(mi.IsSpecialName); // true
//                Debug.Log(mi.DeclaringType); // UnityEngine.Object
//                Debug.Log(mi.DeclaringType == textSource.GetType()); // false
//            }
//        }

//        Get PropertyMethod
//        MethodInfo m = obj.GetType().GetProperty("Value").GetGetMethod();

        var toStringMethods = obj.GetType().GetMethods().Where(mi =>
//                mi.IsSpecialName == false &&
//                mi.DeclaringType == obj.GetType() &&
//                mi.GetParameters().Select(q => q.ParameterType).SequenceEqual(new[] {typeof(int)}) &&
                mi.GetParameters().Length == 0 &&
                mi.ReturnType == typeof(string))
            .Select(mi => mi.Name)
            .ToArray();
        return toStringMethods;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty objProp = property.FindPropertyRelative("obj");
        SerializedProperty nameProp = property.FindPropertyRelative("method");

        label = EditorGUI.BeginProperty(position, label, property);
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        EditorGUI.BeginChangeCheck();
        DrawPropertyField(position, label, objProp, nameProp);
        if (EditorGUI.EndChangeCheck()) property.serializedObject.ApplyModifiedProperties();

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    private void DrawPropertyField(Rect position, GUIContent label,
        SerializedProperty objProp, SerializedProperty nameProp)
    {
        position = EditorGUI.PrefixLabel(position, label);

        position.height = EditorGUI.GetPropertyHeight(objProp);
        EditorGUI.PropertyField(position, objProp, GUIContent.none);
        position.y += position.height + EditorGUIUtility.standardVerticalSpacing;

        if (objProp.objectReferenceValue == null)
        {
            DrawDisabledPopup(position);
            nameProp.stringValue = null;
        }
        else
        {
            string[] methods = toStringMethods(objProp.objectReferenceValue);
            if (methods.Length == 0)
            {
                DrawDisabledPopup(position);
                nameProp.stringValue = null;
            }
            else
            {
                int idx = EditorGUI.Popup(position, Array.IndexOf(methods, nameProp.stringValue), methods);
                nameProp.stringValue = idx >= 0 ? methods[idx] : null;
            }
        }
    }

    private void DrawDisabledPopup(Rect position)
    {
        GUI.enabled = false;
        EditorGUI.Popup(position, -1, new string[] { });
        GUI.enabled = true;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (EditorGUIUtility.singleLineHeight * 2) + EditorGUIUtility.standardVerticalSpacing;
    }
}