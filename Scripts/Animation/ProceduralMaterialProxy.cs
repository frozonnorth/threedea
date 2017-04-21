using UnityEngine;
using System;

public class ProceduralMaterialProxy : MonoBehaviour {
    [Serializable]
    private abstract class Parameter<T> {
        public string name;
        public T value;

        public abstract void Set(ProceduralMaterial pm);
    }
    
    [Serializable]
    private class Float2 : Parameter<Vector2> {
        public override void Set(ProceduralMaterial pm) {
            pm.SetProceduralVector(name, value);
        }
    }
    
    [Serializable]
    private class Int : Parameter<int> {
        public override void Set(ProceduralMaterial pm) {
            pm.SetProceduralEnum(name, value);
        }
    }
    
    [SerializeField]
    Float2[] float2s;
    [SerializeField]
    Int[] ints;


    void OnValidate() {
        Renderer renderer = GetComponent<Renderer>();
        if (!renderer) {
            return;
        }
        ProceduralMaterial material = renderer.material as ProceduralMaterial;
        if (!material) {
            return;
        }
        
        for (int i = 0; i < float2s.Length; ++i) {
            float2s[i].Set(material);
        }
        for (int i = 0; i < ints.Length; ++i) {
            ints[i].Set(material);
        }

        material.RebuildTexturesImmediately();
    }
}
