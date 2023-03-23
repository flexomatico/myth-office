using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(DialogueManager), true)]
public class DialogueManagerEditor : Editor
{
    private SerializedProperty dialogue;
    private SerializedProperty colliderRadius;
    private SerializedProperty NeedsPrerequisites;
    private SerializedProperty FulfillsPrerequisites;
    private SerializedProperty deleteAfterFinished;

    private void OnEnable()
    {
        dialogue = serializedObject.FindProperty("dialogue");
        colliderRadius = serializedObject.FindProperty("colliderRadius");
        NeedsPrerequisites = serializedObject.FindProperty("NeedsPrerequisites");
        FulfillsPrerequisites = serializedObject.FindProperty("FulfillsPrerequisites");
        deleteAfterFinished = serializedObject.FindProperty("deleteAfterFinished");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(dialogue);
        EditorGUILayout.PropertyField(deleteAfterFinished);
        EditorGUILayout.PropertyField(colliderRadius);
        EditorGUILayout.PropertyField(NeedsPrerequisites, true);
        EditorGUILayout.PropertyField(FulfillsPrerequisites, true);
        
        serializedObject.ApplyModifiedProperties();
    }
}
