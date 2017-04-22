using UnityEngine;
using UnityEditor;
using ProceduralAnimation;
using System.Linq;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(ProceduralMaterialAnimation))]
public class ProceduralMaterialPropertyEditor : Editor {
    protected ProceduralMaterial material;
    protected ProceduralMaterialAnimation tgt;
    GUILayoutOption curveWidth = GUILayout.Width(200);
    GUILayoutOption curveHeight = GUILayout.Height(200);

    Color[] colors = new[] {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow
    };

    public override void OnInspectorGUI() {
        tgt = target as ProceduralMaterialAnimation;
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

        var descriptions = material.GetProceduralPropertyDescriptions();
        var properties = tgt.properties;

        var unused = (from description in material.GetProceduralPropertyDescriptions()
                      where description.name[0] != '$' //Not expose built-in properties
                      where properties.All(p => p.name != description.name)
                      select description).ToArray();

        //Add property via dropdown
        var names = unused.Select(p => p.name).Union(new[] { "NONE" }).ToArray();
        int selected = unused.Count();
        EditorGUI.BeginChangeCheck();
        selected = EditorGUILayout.Popup(selected, names);
        if (EditorGUI.EndChangeCheck()) {
            var description = unused[selected];
            RecordForUndo(material, "Add property" + description.name + "in " + tgt.name);
            ProceduralMaterialProperty property;
            switch (description.type) {
                case ProceduralPropertyType.Boolean:
                    property = new ProceduralMaterialBoolean(description.name);
                    break;
                case ProceduralPropertyType.Float:
                    property = new ProceduralMaterialVector(description.name);
                    break;
                case ProceduralPropertyType.Vector2:
                case ProceduralPropertyType.Vector3:
                case ProceduralPropertyType.Vector4:
                    property = new ProceduralMaterialVector(description.name, description.componentLabels);
                    break;
                case ProceduralPropertyType.Color3:
                case ProceduralPropertyType.Color4:
                case ProceduralPropertyType.Enum:
                case ProceduralPropertyType.Texture:
                default:
                    property = new ProceduralMaterialBoolean("NotImplemented: " + description.type);
                    break;
            }
            for (int i = 0; i < property.curves.Length; ++i) {
                property.curves[i] = AnimationCurve.EaseInOut(0, description.minimum, 1, description.maximum);
            }
            ArrayUtility.Add(ref tgt.properties, property);
            EditorUtility.SetDirty(tgt);
            return;
        }

        for (int i = 0; i < properties.Length; ++i) {
            var property = properties[i];
            for (int j = 0; j < descriptions.Length; ++j) {
                var description = descriptions[j];
                if (property.name == description.name) {
                    InputGUI(property, description);
                    break;
                }
            }
            if (GUILayout.Button("Remove")) {
                RecordForUndo(material, "Remove property " + property.name + " in " + tgt.name);
                tgt.properties = tgt.properties.Where(p => p.name != property.name).ToArray();
                break;
            }
        }
    }

    private void InputGUI(ProceduralMaterialProperty property, ProceduralPropertyDescription description) {
        GUI.skin.window.alignment = TextAnchor.MiddleCenter;
        ProceduralPropertyType type = description.type;
        GUIContent content = new GUIContent(description.label, description.name);
        switch (type) {
            case ProceduralPropertyType.Boolean: {
                    DrawLabel(content);
                    EditorGUI.BeginChangeCheck();
                    CurveField(ref property.curves[0], description, Color.green);
                    if (EditorGUI.EndChangeCheck()) {
                        RecordForUndo(material, "Modified property " + description.name + " for material " + material.name);
                        property.SetProperty(material, tgt.time);
                    }
                    break;
                }
            case ProceduralPropertyType.Float: {
                    DrawLabel(content);
                    EditorGUI.BeginChangeCheck();
                    CurveField(ref property.curves[0], description, Color.green);
                    if (EditorGUI.EndChangeCheck()) {
                        RecordForUndo(material, "Modified property " + description.name + " for material " + material.name);
                        property.SetProperty(material, tgt.time);
                    }
                    break;
                }
            case ProceduralPropertyType.Vector2:
            case ProceduralPropertyType.Vector3:
            case ProceduralPropertyType.Vector4: {
                    DrawLabel(content);

                    EditorGUILayout.BeginHorizontal();
                    ProceduralMaterialVector vector = property as ProceduralMaterialVector;
                    if (vector == null) { //HACK: restore hierarchy information that was not serialized
                        vector = new ProceduralMaterialVector(property);
                        int index = ArrayUtility.FindIndex(tgt.properties, p => p == property);
                        tgt.properties[index] = vector;
                        EditorUtility.SetDirty(tgt);
                    }
                    for (int i = 0; i < (int)type; i++) {
                        AnimationCurve curve = vector.curves[i];
                        Color color = colors[i];

                        EditorGUILayout.BeginVertical();
                        GUILayout.Label(description.componentLabels[i]);
                        EditorGUI.BeginChangeCheck();
                        CurveField(ref vector.curves[i], description, color);
                        if (EditorGUI.EndChangeCheck()) {
                            RecordForUndo(material, "Modified property " + description.name + " for material " + material.name);
                            EditorUtility.SetDirty(target);
                            property.SetProperty(material, tgt.time);
                        }

                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();
                    break;
                }
            case ProceduralPropertyType.Color3:
            case ProceduralPropertyType.Color4: {
                    EditorGUI.BeginChangeCheck();
                    Color value3 = EditorGUILayout.ColorField(content, material.GetProceduralColor(description.name));
                    if (EditorGUI.EndChangeCheck()) {
                        RecordForUndo(material, "Modified property " + description.name + " for material " + material.name);
                        material.SetProceduralColor(description.name, value3);
                    }
                    break;
                }
            case ProceduralPropertyType.Enum: {
                    GUIContent[] array = new GUIContent[description.enumOptions.Length];
                    for (int j = 0; j < array.Length; j++) {
                        array[j] = new GUIContent(description.enumOptions[j]);
                    }
                    EditorGUI.BeginChangeCheck();
                    int selected = EditorGUILayout.Popup(content, material.GetProceduralEnum(description.name), array);
                    if (EditorGUI.EndChangeCheck()) {
                        RecordForUndo(material, "Modified property " + description.name + " for material " + material.name);
                        material.SetProceduralEnum(description.name, selected);
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
                    Texture2D texture = EditorGUI.ObjectField(rect, description.name, material.GetProceduralTexture(description.name), typeof(Texture2D), false) as Texture2D;
                    EditorGUILayout.EndHorizontal();
                    if (EditorGUI.EndChangeCheck()) {
                        RecordForUndo(material, "Modified property " + description.name + " for material " + material.name);
                        material.SetProceduralTexture(description.name, texture);
                    }
                    break;
                }
        }
    }

    protected void RecordForUndo(ProceduralMaterial obj, string message) {
        Undo.RecordObject(obj, message);
    }

    protected void CurveField(ref AnimationCurve curve, ProceduralPropertyDescription description, Color color) {
        if (description.hasRange) {
            float min = description.minimum;
            Rect rect = new Rect(0, min, 1, description.maximum - min);
            curve = EditorGUILayout.CurveField(curve, color, rect, curveWidth, curveHeight);
        } else {
            curve = EditorGUILayout.CurveField(curve, curveWidth, curveHeight);
        }


        EditorGUI.BeginChangeCheck();
        tgt.time = EditorGUILayout.Slider(tgt.time, 0, 1, curveWidth);
        if (EditorGUI.EndChangeCheck()) {
            EditorUtility.SetDirty(target);
        }
    }

    protected void DrawLabel(GUIContent content) {
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label(content);
        GUI.skin.label.alignment = TextAnchor.MiddleLeft;
    }
}
