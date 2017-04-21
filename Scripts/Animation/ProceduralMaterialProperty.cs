using UnityEngine;

namespace ProceduralAnimation {
    [ExecuteInEditMode]
    public abstract class ProceduralMaterialProperty<T> : MonoBehaviour {
        [HideInInspector]
        public string propertyName = "";

        //Serialize in a value supported by the editor
        [HideInInspector]
        public T value;

        //TODO public bool cacheHint = true;

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

        public abstract ProceduralPropertyType Type { get; }

        public abstract bool UpdateProperty();

        void Update() {
            if (!Material) {
                return;
            }

            if (UpdateProperty()) {
                Material.RebuildTextures();
            }
        }
    }
}
