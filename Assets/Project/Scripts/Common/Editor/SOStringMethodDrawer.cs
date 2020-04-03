using System;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[CustomPropertyDrawer(typeof(SOStringMethod))]
public class SOStringMethodDrawer : PropertyDrawer
{
    private string[] toStringMethods(Object obj)
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

//    Reference
//    https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/Inspector/UnityEventDrawer.cs
    private string[] toStringMethods2(Object obj)
    {
        var component = obj as Component;
        if (component != null) obj = component.gameObject;

        // empty first list item (none)
        // if obj is null, return
        // separator
        if (obj == null) return new string[] { };

        // obj to go
        // go's components (check for duplicate components)
        //    include namespace in case same name, different namespace
        // use full name

//        UnityEventDrawer

        var methods = toStringMethods(obj);
        foreach (string s in methods)
        {
            Debug.Log(obj.GetType().Name + " " + obj.GetType().FullName + " " + s);
        }

        // add to menu
//        string.IsNullOrEmpty()

        var gameObject = obj as GameObject;
        if (gameObject != null)
        {
            Component[] comps = gameObject.GetComponents<Component>();
            foreach (Component comp in comps)
            {
                if (comp == null)
                    continue;
                var toStringMethodsComp = toStringMethods(comp);
                foreach (string s in toStringMethodsComp)
                {
                    Debug.Log(comp.GetType().Name + " " + comp.GetType().FullName + " " + s);
                }
            }
        }

        return methods;
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

//            toStringMethods2(objProp.objectReferenceValue);
//            if (GUI.Button(functionRect, buttonContent, EditorStyles.popup))
//                BuildPopupList(listenerTarget.objectReferenceValue, m_DummyEvent, pListener).DropDown(functionRect);
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
        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUI.Popup(position, -1, new string[] { });
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (EditorGUIUtility.singleLineHeight * 2) + EditorGUIUtility.standardVerticalSpacing;
    }
}