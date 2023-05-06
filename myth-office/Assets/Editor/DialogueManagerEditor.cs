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
    private SerializedProperty interactionPromptOffset;
    private SerializedProperty NeedsPrerequisites;
    private SerializedProperty FulfillsPrerequisites;
    private SerializedProperty deleteAfterFinished;
    private SerializedProperty doAfterFinished;
    private SerializedProperty doBeforeStarted;

    private bool showDoAfterFinished = false;
    private bool showDoBeforeStarted = false;

    private void OnEnable()
    {
        dialogue = serializedObject.FindProperty("dialogue");
        colliderRadius = serializedObject.FindProperty("colliderRadius");
        interactionPromptOffset = serializedObject.FindProperty("interactionPromptOffset");
        NeedsPrerequisites = serializedObject.FindProperty("NeedsPrerequisites");
        FulfillsPrerequisites = serializedObject.FindProperty("FulfillsPrerequisites");
        deleteAfterFinished = serializedObject.FindProperty("deleteAfterFinished");
        doAfterFinished = serializedObject.FindProperty("doAfterFinished");
        doBeforeStarted = serializedObject.FindProperty("doBeforeStarted");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(dialogue);
        EditorGUILayout.PropertyField(deleteAfterFinished);
        EditorGUILayout.PropertyField(interactionPromptOffset);
        EditorGUILayout.PropertyField(colliderRadius);
        EditorGUILayout.PropertyField(NeedsPrerequisites, true);
        EditorGUILayout.PropertyField(FulfillsPrerequisites, true);
        showDoAfterFinished = EditorGUILayout.BeginFoldoutHeaderGroup(showDoAfterFinished, "Do After Finished");
        if (showDoAfterFinished)
        {
            EditorGUILayout.PropertyField(doAfterFinished, true);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        showDoBeforeStarted = EditorGUILayout.BeginFoldoutHeaderGroup(showDoBeforeStarted, "Do Before Started");
        if (showDoBeforeStarted)
        {
            EditorGUILayout.PropertyField(doBeforeStarted, true);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        
        serializedObject.ApplyModifiedProperties();
    }
}
