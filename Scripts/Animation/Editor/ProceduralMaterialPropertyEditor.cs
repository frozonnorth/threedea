using UnityEngine;
using UnityEditor;
using ProceduralAnimation;
using System.Linq;
using System.Collections.Generic;

public class ProceduralMaterialPropertyEditor : Editor {
    protected ProceduralMaterial material = null;

    public override void OnInspectorGUI() {
        ProceduralMaterialVector tgt = target as ProceduralMaterialVector;
        material = tgt.Material;

        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        if (EditorGUI.EndChangeCheck()) {
            EditorUtility.SetDirty(tgt);
            return;
        }

        if (!material) {
            return;
        }

        var descriptions = (from description in material.GetProceduralPropertyDescriptions()
                            where description.type == tgt.Type
                            where description.name[0] != '$' //Not expose built-in properties
                            select description).ToArray();

        //Select property via dropdown
        var names = descriptions.Select(p => p.name).Union(new[] { "NONE" }).ToArray();
        int selected = 0;
        while (selected < descriptions.Length && names[selected] != tgt.propertyName) {
            ++selected;
        }
        EditorGUI.BeginChangeCheck();
        selected = EditorGUILayout.Popup(selected, names);
        if (EditorGUI.EndChangeCheck()) {
            RecordForUndo(material, "Change property in " + tgt.name);
            tgt.propertyName = descriptions[selected].name;
            EditorUtility.SetDirty(tgt);
        }

        //Expose the selected material properties
        if (selected < descriptions.Length) {
            InputGUI(descriptions[selected]);
        }
    }

    protected virtual void InputGUI(ProceduralPropertyDescription input) {
        ProceduralPropertyType type = input.type;
        GUIContent content = new GUIContent(input.label, input.name);
        switch (type) {
            case ProceduralPropertyType.Boolean: {
                    EditorGUI.BeginChangeCheck();
                    bool value = EditorGUILayout.Toggle(content, material.GetProceduralBoolean(input.name));
                    if (EditorGUI.EndChangeCheck()) {
                        RecordForUndo(material, "Modified property " + input.name + " for material " + material.name);
                        material.SetProceduralBoolean(input.name, value);
                    }
                    break;
                }
            case ProceduralPropertyType.Float: {
                    EditorGUI.BeginChangeCheck();
                    float value2;
                    if (input.hasRange) {
                        float minimum = input.minimum;
                        float maximum = input.maximum;
                        value2 = EditorGUILayout.Slider(content, material.GetProceduralFloat(input.name), minimum, maximum);
                    } else {
                        value2 = EditorGUILayout.FloatField(content, material.GetProceduralFloat(input.name));
                    }
                    if (EditorGUI.EndChangeCheck()) {
                        RecordForUndo(material, "Modified property " + input.name + " for material " + material.name);
                        material.SetProceduralFloat(input.name, value2);
                    }
                    break;
                }
            case ProceduralPropertyType.Vector2:
            case ProceduralPropertyType.Vector3:
            case ProceduralPropertyType.Vector4: {
                    int num = (int) type;
                    Vector4 vector = material.GetProceduralVector(input.name);
                    EditorGUI.BeginChangeCheck();
                    if (input.hasRange) {
                        float minimum2 = input.minimum;
                        float maximum2 = input.maximum;
                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(EditorGUI.indentLevel * 15f);
                        GUILayout.Label(content);
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.indentLevel++;
                        for (int i = 0; i < num; i++) {
                            vector[i] = EditorGUILayout.Slider(new GUIContent(input.componentLabels[i]), vector[i], minimum2, maximum2);
                        }
                        EditorGUI.indentLevel--;
                        EditorGUILayout.EndVertical();
                    } else {
                        switch (num) {
                            case 4:
                                vector = EditorGUILayout.Vector4Field(input.name, vector);
                                break;
                            case 3:
                                vector = EditorGUILayout.Vector3Field(input.name, vector);
                                break;
                            case 2:
                                vector = EditorGUILayout.Vector2Field(input.name, vector);
                                break;
                        }
                    }
                    if (EditorGUI.EndChangeCheck()) {
                        RecordForUndo(material, "Modified property " + input.name + " for material " + material.name);
                        material.SetProceduralVector(input.name, vector);
                    }
                    break;
                }
            case ProceduralPropertyType.Color3:
            case ProceduralPropertyType.Color4: {
                    EditorGUI.BeginChangeCheck();
                    Color value3 = EditorGUILayout.ColorField(content, material.GetProceduralColor(input.name));
                    if (EditorGUI.EndChangeCheck()) {
                        RecordForUndo(material, "Modified property " + input.name + " for material " + material.name);
                        material.SetProceduralColor(input.name, value3);
                    }
                    break;
                }
            case ProceduralPropertyType.Enum: {
                    GUIContent[] array = new GUIContent[input.enumOptions.Length];
                    for (int j = 0; j < array.Length; j++) {
                        array[j] = new GUIContent(input.enumOptions[j]);
                    }
                    EditorGUI.BeginChangeCheck();
                    int selected = EditorGUILayout.Popup(content, material.GetProceduralEnum(input.name), array);
                    if (EditorGUI.EndChangeCheck()) {
                        RecordForUndo(material, "Modified property " + input.name + " for material " + material.name);
                        material.SetProceduralEnum(input.name, selected);
                    }
                    break;
                }
            case ProceduralPropertyType.Texture: {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(EditorGUI.indentLevel * 15f);
                    GUILayout.Label(content);
                    GUILayout.FlexibleSpace();
                    Rect rect = GUILayoutUtility.GetRect(64f, 64f, GUILayout.ExpandWidth(false));
                    EditorGUI.BeginChangeCheck();
                    Texture2D texture = EditorGUI.ObjectField(rect, input.name, material.GetProceduralTexture(input.name), typeof(Texture2D), false) as Texture2D;
                    EditorGUILayout.EndHorizontal();
                    if (EditorGUI.EndChangeCheck()) {
                        RecordForUndo(material, "Modified property " + input.name + " for material " + material.name);
                        material.SetProceduralTexture(input.name, texture);
                    }
                    break;
                }
        }
    }

    protected void RecordForUndo(ProceduralMaterial material, string message) {
        Undo.RecordObject(material, message);
    }
}
