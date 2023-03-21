using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

public class EditPrerequisitesAttribute : PropertyAttribute
{
    public Type myType;
    public string propertyName;
    public SerializedObject serializedObject;
    
    public EditPrerequisitesAttribute(Type _myType, string _propertyName)
    {
        myType = _myType;
        propertyName = _propertyName;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(EditPrerequisitesAttribute))]
public class EditPrerequisitesDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditPrerequisitesAttribute atb = attribute as EditPrerequisitesAttribute;
        List<string> stringList = null;

        if (atb.myType.GetField(atb.propertyName) != null)
        {
            stringList = atb.myType.GetField("prerequisites").GetValue(atb.myType) as List<string>;
            Debug.Log(property.arraySize);
        }

        EditorGUI.PropertyField(position, property, label);
    }
}
#endif