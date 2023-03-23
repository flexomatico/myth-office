using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "PrerequisiteList", menuName = "PrerequisiteList", order = 1)]
public class Prerequisites : ScriptableObject
{
    public List<string> prerequisites;
}
