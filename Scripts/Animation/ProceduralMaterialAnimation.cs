using UnityEngine;
using System.Collections.Generic;
using System;

namespace ProceduralAnimation {
    [ExecuteInEditMode]
    public class ProceduralMaterialAnimation : MonoBehaviour {
        [Range(0,1)]
        public float time = 0;
        private ProceduralMaterial _material;

        public ProceduralMaterial Material {
            get {
                if (!_material) {
                    Renderer renderer = GetComponent<Renderer>();
                    if (renderer) {
                        _material = renderer.sharedMaterial as ProceduralMaterial;
                    }
                }
                return _material;
            }
        }

        public ProceduralMaterialProperty[] properties = new ProceduralMaterialProperty[0];

        void Update() {
            if (!Material) {
                return;
            }

            bool changed = false;
            int length = properties.Length;
            for (int i = 0; i < length; ++i) {
                changed |= properties[i].SetProperty(Material, time);
            }
            if (changed) {
                Material.RebuildTextures();
            }
        }
    }
}