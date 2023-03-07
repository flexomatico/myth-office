using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Codice.Client.BaseCommands;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEditorInternal;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(Dialogue), true)]
public class DialogueEditor : Editor
{
    #region Private Attributes

    private static readonly Color ProSkinTextColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);
    private static readonly Color PersonalSkinTextColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);

    private static readonly Color ProSkinSelectionBgColor = new Color(44.0f / 255.0f, 93.0f / 255.0f, 135.0f / 255.0f, 1.0f);
    private static readonly Color PersonalSkinSelectionBgColor = new Color(58.0f / 255.0f, 114.0f / 255.0f, 176.0f / 255.0f, 1.0f);

    private const float AdditionalSpaceMultiplier = 1.0f;

    private const float HeightHeader = 20.0f;
    private const float MarginReorderIcon = 20.0f;
    private const float ShrinkHeaderWidth = 15.0f;
    private const float XShiftHeaders = 15.0f;

    private const float npcResponseElementHeight = 155.0f;
    private const float playerResponseElementHeight = 185.0f;

    private GUIStyle headersStyle;

    private ReorderableList list;

    #endregion

    #region Editor Methods

    
    private void OnEnable()
    {
        headersStyle = new GUIStyle();
        headersStyle.alignment = TextAnchor.MiddleLeft;
        headersStyle.normal.textColor = EditorGUIUtility.isProSkin ? ProSkinTextColor : PersonalSkinTextColor;
        headersStyle.fontStyle = FontStyle.Bold;
        
        list = new ReorderableList(serializedObject, serializedObject.FindProperty("dialogueParts"), true, true, true, true);
        list.drawHeaderCallback += OnDrawReorderListHeader;
        list.drawElementCallback += OnDrawListElement;
        list.drawElementBackgroundCallback += OnDrawReorderListBg;
        list.elementHeightCallback += OnReorderListElementHeight;
        list.onAddDropdownCallback += OnReorderListAddDropdown;
    }

    private void OnDisable()
    {
        list.drawElementCallback -= OnDrawListElement;
        list.elementHeightCallback -= OnReorderListElementHeight;
        list.drawElementBackgroundCallback -= OnDrawReorderListBg;
        list.drawHeaderCallback -= OnDrawReorderListHeader;
        list.onAddDropdownCallback -= OnReorderListAddDropdown;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        list.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }

    #endregion

    #region ReorderableList Callbacks

    private void OnDrawListElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        int length = list.serializedProperty.arraySize;

        if (length <= 0)
            return;
        
        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
        SerializedProperty dialogueType = element.FindPropertyRelative("dialogueType");
        string dialogueTypeName = dialogueType.enumDisplayNames[dialogueType.enumValueIndex];

        Rect foldoutHeaderRect = rect;
        foldoutHeaderRect.height = HeightHeader;
        foldoutHeaderRect.x += XShiftHeaders;
        element.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(foldoutHeaderRect, element.isExpanded, dialogueTypeName);
        EditorGUI.indentLevel++;

        if (element.isExpanded)
        {

            EditorGUILayout.BeginVertical();
            
            if ((DialogueType)dialogueType.enumValueFlag == DialogueType.NPCResponse)
            {
                rect.y += GetDefaultSpaceBetweenElements();
                rect.height = EditorGUIUtility.singleLineHeight * 4.0f;
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("line"), new GUIContent("Line"));

                rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("leftImage"), new GUIContent("Left Image"));
                rect.y += GetDefaultSpaceBetweenElements();
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("rightImage"), new GUIContent("Right Image"));
                rect.y += GetDefaultSpaceBetweenElements();
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("sound"), new GUIContent("Sound"));

            } 
            else if ((DialogueType)dialogueType.enumValueFlag == DialogueType.PlayerResponse)
            {
                rect.height = EditorGUIUtility.singleLineHeight * 3.0f;
                rect.y += GetDefaultSpaceBetweenElements();
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("choice1"), new GUIContent("Choice 1"));
                rect.y += rect.height;
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("choice2"), new GUIContent("Choice 2"));
                rect.y += rect.height;
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("choice3"), new GUIContent("Choice 3"));
            }
            EditorGUILayout.EndVertical();
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
    
    private void OnDrawReorderListHeader(Rect rect)
    {
        EditorGUI.LabelField(rect, "Dialogue Parts");
    }

    private void OnDrawReorderListElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        int length = list.serializedProperty.arraySize;

        if (length <= 0)
            return;

        SerializedProperty iteratorProp = list.serializedProperty.GetArrayElementAtIndex(index);

        SerializedProperty actionTypeParentProp = iteratorProp.FindPropertyRelative("dialogueType");
        string actionName = actionTypeParentProp.enumDisplayNames[actionTypeParentProp.enumValueIndex];

        Rect labelfoldRect = rect;
        labelfoldRect.height = HeightHeader;
        labelfoldRect.x += XShiftHeaders;
        labelfoldRect.width -= ShrinkHeaderWidth;

        iteratorProp.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(labelfoldRect, iteratorProp.isExpanded, actionName);

        if (iteratorProp.isExpanded)
        {
            ++EditorGUI.indentLevel;

            SerializedProperty endProp = iteratorProp.GetEndProperty();

            int i = 0;
            while (iteratorProp.NextVisible(true) && !EqualContents(endProp, iteratorProp))
            {
                float multiplier = i == 0 ? AdditionalSpaceMultiplier : 1.0f;
                rect.y += GetDefaultSpaceBetweenElements() * multiplier;
                rect.height = EditorGUIUtility.singleLineHeight;

                EditorGUI.PropertyField(rect, iteratorProp, true);

                ++i;
            }

            --EditorGUI.indentLevel;
        }

        EditorGUI.EndFoldoutHeaderGroup();
    }

    private void OnDrawReorderListBg(Rect rect, int index, bool isActive, bool isFocused)
    {
        if (!isFocused || !isActive)
            return;

        float height = OnReorderListElementHeight(index);

        SerializedProperty prop = list.serializedProperty.GetArrayElementAtIndex(index);

        // remove a bit of the line that goes beyond the header label
        if (!prop.isExpanded)
            height -= EditorGUIUtility.standardVerticalSpacing;

        Rect copyRect = rect;
        copyRect.width = MarginReorderIcon;
        copyRect.height = height;

        // draw two rects indepently to avoid overlapping the header label
        Color color = EditorGUIUtility.isProSkin ? ProSkinSelectionBgColor : PersonalSkinSelectionBgColor;
        EditorGUI.DrawRect(copyRect, color);

        float offset = 2.0f;
        rect.x += MarginReorderIcon;
        rect.width -= (MarginReorderIcon + offset);

        rect.height = height - HeightHeader + offset;
        rect.y += HeightHeader - offset;

        EditorGUI.DrawRect(rect, color);
    }

    private float OnReorderListElementHeight(int index)
    {
        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
        DialogueType dialogueType = (DialogueType)element.FindPropertyRelative("dialogueType").enumValueFlag;
        if (dialogueType == DialogueType.NPCResponse)
            return element.isExpanded ? npcResponseElementHeight : HeightHeader;
        else if (dialogueType == DialogueType.PlayerResponse)
            return element.isExpanded ? playerResponseElementHeight : HeightHeader;
        return 0.0f;
    }

    private void OnReorderListAddDropdown(Rect buttonRect, ReorderableList list)
    {
        GenericMenu menu = new GenericMenu();
        List<Type> showTypes = GetNonAbstractTypesSubclassOf<DialoguePart>();

        for (int i = 0; i < showTypes.Count; ++i)
        {
            Type type = showTypes[i];
            string actionName = showTypes[i].Name;

            InsertSpaceBeforeR(ref actionName);
            menu.AddItem(new GUIContent(actionName), false, OnAddItemFromDropdown, (object)type);
        }

        menu.ShowAsContext();
    }

    private void OnAddItemFromDropdown(object obj)
    {
        Type settingsType = (Type)obj;

        int last = list.serializedProperty.arraySize;
        list.serializedProperty.InsertArrayElementAtIndex(last);

        SerializedProperty lastProp = list.serializedProperty.GetArrayElementAtIndex(last);
        lastProp.managedReferenceValue = Activator.CreateInstance(settingsType);

        serializedObject.ApplyModifiedProperties();
    }

    #endregion

    #region Helper Methods

    private float GetDefaultSpaceBetweenElements()
    {
        return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
    }

    private void InsertSpaceBeforeR(ref string theString)
    {
        for (int i = 0; i < theString.Length; ++i)
        {
            char currChar = theString[i];

            if (currChar == 'R')
            {
                theString = theString.Insert(i, " ");
                ++i;
            }
        }
    }

    private bool EqualContents(SerializedProperty a, SerializedProperty b)
    {
        return SerializedProperty.EqualContents(a, b);
    }

    private List<Type> GetNonAbstractTypesSubclassOf<T>(bool sorted = true) where T : class
    {
        Type parentType = typeof(T);
        Assembly assembly = Assembly.GetAssembly(parentType);

        List<Type> types = assembly.GetTypes().Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(parentType)).ToList();

        if (sorted)
            types.Sort(CompareTypesNames);

        return types;
    }

    private int CompareTypesNames(Type a, Type b)
    {
        return a.Name.CompareTo(b.Name);
    }

    private bool DoesReordListHaveElementOfType(string type)
    {
        for (int i = 0; i < list.serializedProperty.arraySize; ++i)
        {
            // this works but feels ugly. Type in the array element looks like "managedReference<actualStringType>"
            if (list.serializedProperty.GetArrayElementAtIndex(i).type.Contains(type))
                return true;
        }

        return false;
    }
}

#endregion