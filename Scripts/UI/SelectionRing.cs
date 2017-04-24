#if UNITY_EDITOR

using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEditor.Experimental.EditorVR.Modules;
using UnityEditor.Experimental.EditorVR.UI;
using UnityEditor.Experimental.EditorVR.Utilities;
using System.Collections;

namespace Threedea {
    namespace UI {
        [ExecuteInEditMode]
        internal class SelectionRing : MonoBehaviour, IRayEnterHandler, IRayExitHandler, IRayHoverHandler {
            [Serializable]
            private class Option {
                public string label;
                public Sprite icon; //IDEA use this to dynamically instantiate buttons
                public SpriteRenderer instance;
                public UnityAction action;
                public bool enabled = true;
            }

            [SerializeField]
            private GameObject buttonPrefab;
            [SerializeField]
            private Renderer renderer;

            #region ProceduralParameter
            [SerializeField]
            [Range(0.1f, 1f)]
            private float pIconRadius = 1f;
            [SerializeField]
            [Range(-1f, 1f)]
            private float pDiscRadius = 0f;
            [SerializeField]
            [Range(-1f, 1f)]
            private float pDiscRotation = 0f;
            [SerializeField]
            [Range(0f, 0.1f)]
            private float pIconScale = 0.05f;
            [SerializeField]
            [Range(0f, 1f)]
            private float pInterstice = 0f;
            #endregion ProceduralParameter

            [SerializeField]
            private Option[] options;
            private int Length {
                get {
                    return options.Where(o => o.enabled).Count();
                }
            }

            [Tooltip("serialized only for debugging")]
            [SerializeField]
            private float angle = 1.5f;
            private Animator animator;
            private int visibilityHash;

            #region MonoBehaviour
            void Start() {
                animator = GetComponent<Animator>();
                visibilityHash = Animator.StringToHash("visibility");
                animator.SetInteger(visibilityHash, 0);
            }

            void Update() {
                if (!animator.isInitialized) animator.Rebind();
                visibilityHash = Animator.StringToHash("visibility");
                //Position the icons
                int enabledCounter = 0;
                for (int i = 0; i < options.Length; ++i) {
                    Option option = options[i];
                    option.instance.gameObject.SetActive(option.enabled);
                    if (option.enabled) {
                        option.instance.transform.position = GetIconPosition(enabledCounter++);
                        option.instance.transform.localScale = new Vector3(pIconScale, pIconScale, pIconScale);
                    }
                }

                //Set the procedural material parameters
                //IDEA: use MaterialPropertyBlock
                ProceduralMaterial material = renderer.sharedMaterial as ProceduralMaterial;
                material.SetProceduralFloat("Count", enabledCounter);
                material.SetProceduralFloat("Selected", (enabledCounter - 1f) * angle / Mathf.PI);
                material.SetProceduralVector("Offset", new Vector4(pDiscRotation, pDiscRadius, 0, 0));
                material.SetProceduralVector("Interstice", new Vector4(pInterstice, 0, 0, 0));
                material.RebuildTextures();
                renderer.sortingOrder = -100;

            }
            #endregion MonoBehaviour

            void OnDrawGizmos() {
                int lineCount = Length;
                for (int i = 0; i < lineCount; ++i) {
                    Gizmos.DrawLine(transform.position, GetIconPosition(i));
                    Gizmos.DrawWireSphere(GetIconPosition(i), pIconRadius * 0.01f);
                }
            }

            private Vector3 GetIconPosition(int i) {
                float step = Mathf.PI / Length;
                float angle = step * 0.5f + i * step;
                float x = pIconRadius * Mathf.Cos(Mathf.PI - angle);
                float y = pIconRadius * Mathf.Sin(angle);
                return transform.position + new Vector3(x, -y, 0);
            }

            #region EditorVR-IRay
            public void OnRayEnter(RayEventData eventData) {
                Expand();
            }

            public void OnRayHover(RayEventData eventData) {
                RaycastResult raycastResult = eventData.pointerCurrentRaycast;
                Vector3 localPosition = transform.InverseTransformPoint(raycastResult.worldPosition);
                angle = Mathf.Acos(Vector3.Dot(-transform.right, raycastResult.worldPosition));
            }

            public void OnRayExit(RayEventData eventData) {
                Hide();
            }
            #endregion EditorVR-IRay

            public void Hide() {
                animator.SetInteger(visibilityHash, 0);
            }

            public void Dock() {
                animator.SetInteger(visibilityHash, 1);
            }

            public void Expand() {
                animator.SetInteger(visibilityHash, 2);
            }

        }
    }
}

#endif