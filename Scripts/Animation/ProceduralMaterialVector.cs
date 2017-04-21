using UnityEngine;
using System.Collections;
using System;

namespace ProceduralAnimation {
    public class ProceduralMaterialVector : ProceduralMaterialProperty<Vector4> {
        public float resolution = 0.01f;
        //TODO public bool cacheHint = true;

        private enum VectorType {
            Float = ProceduralPropertyType.Float,
            Vector2 = ProceduralPropertyType.Vector2,
            Vector3 = ProceduralPropertyType.Vector3,
            Vector4 = ProceduralPropertyType.Vector4,
        }
        [SerializeField]
        private VectorType _type = VectorType.Float;

        public override ProceduralPropertyType Type {
            get {
                return (ProceduralPropertyType)_type;
            }
        }

        /// <summary>
        /// Updates property using a certain resolution.
        /// This reduces precision of loating points
        /// and should help with caching.
        /// </summary>
        /// <returns>if any value has been changed</returns>
        public override bool UpdateProperty() {
            bool changed = false;
            for (int i = 0; i < (int)_type; ++i) {
                float adjusted = value[i] - value[i] % resolution;
                if (adjusted != value[i]) {
                    value[i] = adjusted;
                    changed = true;
                }
            }
            if (changed) {
                Material.SetProceduralVector(propertyName, value);
            }
            return changed;
        }
    }
}