using UnityEngine;
using UnityEditor;
using ProceduralAnimation;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(ProceduralMaterialVector))]
public class ProceduralMaterialVectorEditor : ProceduralMaterialPropertyEditor {

    protected override void InputGUI(ProceduralPropertyDescription input) {
        ProceduralMaterialVector proxy = target as ProceduralMaterialVector;
        GUIContent content = new GUIContent(input.label, input.name);
        EditorGUI.BeginChangeCheck();
        if (input.hasRange) {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUI.indentLevel * 15f);
            GUILayout.Label(content);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel++;
            for (int i = 0; i < (int)input.type; i++) {
                proxy.value[i] = EditorGUILayout.Slider(new GUIContent(input.componentLabels[i]), proxy.value[i], input.minimum, input.maximum);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        } else {
            switch (input.type) {
                case ProceduralPropertyType.Vector4:
                    proxy.value = EditorGUILayout.Vector4Field(input.name, proxy.value);
                    break;
                case ProceduralPropertyType.Vector3:
                    proxy.value = EditorGUILayout.Vector3Field(input.name, proxy.value);
                    break;
                case ProceduralPropertyType.Vector2:
                    proxy.value = EditorGUILayout.Vector2Field(input.name, proxy.value);
                    break;
                default:
                    if (input.hasRange) {
                        proxy.value[0] = EditorGUILayout.Slider(content, proxy.value[0], input.minimum, input.maximum);
                    } else {
                        proxy.value[0] = EditorGUILayout.FloatField(content, proxy.value[0]);
                    }
                    break;
            }
        }
        if (EditorGUI.EndChangeCheck()) {
            RecordForUndo(material, "Modified property " + input.name + " for material " + material.name);
            EditorUtility.SetDirty(target);
        }
    }
}

