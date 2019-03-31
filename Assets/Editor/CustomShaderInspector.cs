using UnityEngine;
using UnityEditor;
using System;

public class CustomShaderInspector : ShaderGUI
{
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        base.OnGUI(materialEditor, properties); // shows default properties

        Material targetMaterial = materialEditor.target as Material;

        // For OUTLINE_ON
        // check if outline is on, and show a checkbox for it
        bool outline = Array.IndexOf(targetMaterial.shaderKeywords, "OUTLINE_ON") != -1;
        EditorGUI.BeginChangeCheck();
        outline = EditorGUILayout.Toggle("Outline", outline);
        if (EditorGUI.EndChangeCheck())
        {
            // enable or disable the keyword based on checkbox
            if (outline)
                targetMaterial.EnableKeyword("OUTLINE_ON");
            else
                targetMaterial.DisableKeyword("OUTLINE_ON");
        }

        // For DIFFUSE_ON
        // check if outline is on, and show a checkbox for it
        bool diffuse = Array.IndexOf(targetMaterial.shaderKeywords, "DIFFUSE_ON") != -1;
        EditorGUI.BeginChangeCheck();
        diffuse = EditorGUILayout.Toggle("DIFFUSE_ON", diffuse);
        if (EditorGUI.EndChangeCheck())
        {
            // enable or disable the keyword based on checkbox
            if (diffuse)
                targetMaterial.EnableKeyword("DIFFUSE_ON");
            else
                targetMaterial.DisableKeyword("DIFFUSE_ON");
        }
    }
}