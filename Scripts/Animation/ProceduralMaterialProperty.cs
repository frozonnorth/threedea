using System;
using System.Linq;
using UnityEngine;

namespace ProceduralAnimation {
    [Serializable]
    public class ProceduralMaterialProperty {
        public string name = "";

        //TODO public bool cacheHint = true;
        public AnimationCurve[] curves;
        public float resolution = 0.1f;

        public virtual bool SetProperty(ProceduralMaterial material, float t) {
            return false;
        }
        
        public ProceduralMaterialProperty(string name) {
            this.name = name;
        }
        public ProceduralMaterialProperty(ProceduralMaterialProperty other) {
            name = other.name;
            curves = other.curves;
            resolution = other.resolution;

        }
    }

    public class ProceduralMaterialBoolean : ProceduralMaterialProperty {
        public ProceduralMaterialBoolean(string name) : base(name) { }

        public float cutoff;
        public override bool SetProperty(ProceduralMaterial material, float t) {
            bool previous = material.GetProceduralBoolean(name);
            bool current = curves[0].Evaluate(t) > cutoff;

            if (previous != current) {
                material.SetProceduralBoolean(name, current);
                return true;
            }
            return false;
        }
    }

    public class ProceduralMaterialInt : ProceduralMaterialProperty {
        public ProceduralMaterialInt(string name) : base(name) { }

        public override bool SetProperty(ProceduralMaterial material, float t) {
            int previous = material.GetProceduralEnum(name);
            int current = Mathf.RoundToInt(curves[0].Evaluate(t));

            if (previous != current) {
                material.SetProceduralEnum(name, current);
                return true;
            }
            return false;
        }
    }

    public class ProceduralMaterialVector : ProceduralMaterialProperty {
        public ProceduralMaterialVector(string name, string[] names = null) : base(name) {
            names = names ?? new [] { "" };
            curves = names.Select(n => AnimationCurve.Linear(0,0,1,1)).ToArray();
        }

        public ProceduralMaterialVector(ProceduralMaterialProperty property):base(property) { }

        /// <summary>
        /// Updates property using a certain resolution.
        /// This reduces precision of floating points
        /// and should help with caching.
        /// </summary>
        /// <returns>if any value has been changed</returns>
        public override bool SetProperty(ProceduralMaterial material, float t) {
            Vector4 previous = material.GetProceduralVector(name);
            Vector4 current = new Vector4(
                BucketEvaluate(t, 0),
                curves.Length > 1 ? BucketEvaluate(t, 1) : 0,
                curves.Length > 2 ? BucketEvaluate(t, 2) : 0,
                curves.Length > 3 ? BucketEvaluate(t, 3) : 0
            );

            if (previous != current) {
                material.SetProceduralVector(name, current);
                return true;
            }
            return false;
        }

        public float BucketEvaluate(float t, int index = 0) {
            float value = curves[index].Evaluate(t);
            return value - value % resolution;
        }
    }
}
