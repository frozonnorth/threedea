#if UNITY_EDITOR && UNITY_EDITORVR

using UnityEngine;
using UnityEditor.Experimental.EditorVR;
using UnityEditor.Experimental.EditorVR.Utilities;

namespace Threedea.Tools {
    public class IdeaSelectTool : MonoBehaviour, ITool, ISetHighlight {
        [SerializeField]
        private LinkHandle linkPrefab;
        void Start() {
            var ideas = FindObjectsOfType(typeof(Idea));
            foreach (Idea idea in ideas) {
                if (idea.subject && idea.subject != idea) {
                    LinkHandle link = CreateLink(idea);
                }
            }
        }

        void Update() {
            var ideas = FindObjectsOfType(typeof(Idea));
            foreach (Idea obj in ideas) {
                Debug.DrawLine(Vector3.zero, obj.transform.position);
            }
        }

        private LinkHandle CreateLink(Idea idea) {
            var prefab = linkPrefab.gameObject;
            var go = ObjectUtils.Instantiate(prefab, runInEditMode: true);

            go.transform.position = idea.transform.position;
            var link = go.GetComponent<LinkHandle>();
            link.PositionBetween(idea, idea.subject);

            return link;
        }
    }
}

#endif