#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEditor.AssetImporters;

public class FitScaleToMaterialTexture : MonoBehaviour
{
    public float pixelsPerUnit = 100;

    private void OnValidate() {

        // Find the material
        Material material = gameObject.GetComponent<MeshRenderer>().sharedMaterials[0];
        if (material == null) return;
        Texture texture = material.GetTexture("_MainTex");

        // look up the original texture (before it was made a power of 2 texture)
        int width;
        int height;
        string path = AssetDatabase.GetAssetPath(texture);
        ((TextureImporter)TextureImporter.GetAtPath(path)).GetSourceTextureWidthAndHeight(out width, out height);
        
        // calculate scale according to original size
        Vector2 scale = new Vector2(width / pixelsPerUnit, height / pixelsPerUnit);
        gameObject.transform.localScale = scale;

        // make sure the new value is saved
        PrefabUtility.RecordPrefabInstancePropertyModifications(gameObject);
    }
}
#endif