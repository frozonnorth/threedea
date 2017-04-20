#if UNITY_EDITOR

using UnityEngine;
using UnityEditor.Experimental.EditorVR.Modules;
using UnityEditor.Experimental.EditorVR.UI;
using UnityEditor.Experimental.EditorVR.Utilities;
using System;
using UnityEngine.EventSystems;

namespace Threedea {
    internal class LinkHandle : MonoBehaviour, ISelectionFlags, IRayEnterHandler, IRayExitHandler, IRayHoverHandler {
        private float growFactor = 1.25f;
        private LinkStyler styler;
        [SerializeField]
        private GameObject hoverPrefab;
        [SerializeField]
        private GameObject hoverInstance;

        public SelectionFlags selectionFlags { get { return m_SelectionFlags; } set { m_SelectionFlags = value; } }
        [SerializeField]
        [FlagsProperty]
        SelectionFlags m_SelectionFlags = SelectionFlags.Ray | SelectionFlags.Direct;

        public void Start() {
            styler = GetComponent<LinkStyler>();
            hoverInstance = ObjectUtils.Instantiate(hoverPrefab, transform, runInEditMode: true, active: false);
        }

        public void OnRayEnter(RayEventData eventData) {
            hoverInstance.SetActive(true);
        }

        public void OnRayHover(RayEventData eventData) {
            RaycastResult raycastResult = eventData.pointerCurrentRaycast;
            if (raycastResult.gameObject.CompareTag("Link handle")) {
                hoverInstance.transform.position = ClosestPoint(raycastResult.worldPosition);
                hoverInstance.transform.LookAt(CameraUtils.GetMainCamera().transform.position);
            }
        }

        public void OnRayExit(RayEventData eventData) {
            hoverInstance.SetActive(false);
        }

        /// <summary>
        /// The closes point along the axis of this link
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        private Vector3 ClosestPoint(Vector3 worldPosition) {
            Vector3 toPoint = worldPosition - transform.position;
            Vector3 axis = transform.forward;
            Vector3 offset = axis * Vector3.Dot(toPoint, axis);
            return transform.position + offset;
        }

    }
}
#endif
