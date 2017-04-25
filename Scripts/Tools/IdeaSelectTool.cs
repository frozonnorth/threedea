#if UNITY_EDITOR && UNITY_EDITORVR

using UnityEngine;
using UnityEditor.Experimental.EditorVR;
using UnityEditor.Experimental.EditorVR.Utilities;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Threedea.Tools {
    [RequiresTag(ROOT_TAG)]
    public class IdeaSelectTool : MonoBehaviour, ITool, IActions, ISetHighlight, IUsesDirectSelection {
        public const string ROOT_TAG = "RootIdea";

        [SerializeField]
        private GameObject bulbPrefab;
        [SerializeField]
        private LinkHandle linkPrefab;
        private List<List<LinkHandle>> linkMatrix = new List<List<LinkHandle>>() { new List<LinkHandle>() };
        private Idea rootIdea;

        #region Actions
        internal class IdeaAction : IAction, ITooltip {
            public string tooltipText { get; internal set; }
            public Sprite icon { get; internal set; }
            internal Action execute;

            public void ExecuteAction() {
                if (execute != null) {
                    execute();
                }
            }
        }
        private List<IAction> m_Actions;
        readonly IdeaAction addAction = new IdeaAction();
        readonly IdeaAction deleteAction = new IdeaAction();

        public List<IAction> actions {
            get {
                if (m_Actions == null) {
                    m_Actions = new List<IAction>()
                    {
                        addAction,
                        deleteAction
                    };
                }
                return m_Actions;
            }
        }
        private void InitActions() {
            addAction.execute = () => Debug.Log("add");
            deleteAction.execute = () => Debug.Log("delete");
        }
        #endregion

        void Start() {
            InitActions();

        }

        void OnDestroy() {
            foreach (var link in linkMatrix.SelectMany(l => l)) {
                DestroyImmediate(link.gameObject);
            }
        }

        void Update() {
            foreach (var link in linkMatrix.SelectMany(l => l)) {
                link.idea.UpdatePosition();
                link.UpdatePosition();
            }
        }

        void InitIdeas() {
            linkMatrix.Clear();
            GameObject root = GameObject.FindWithTag(ROOT_TAG);
            rootIdea = root.GetComponent<Idea>();
            
            //Prevents initializing an idea twice
            HashSet<Idea> initialized = new HashSet<Idea>() { rootIdea };

            //The root has no links
            List<Idea> parents = new List<Idea>() { rootIdea };

            while (parents.Count > 0) {
                List<LinkHandle> row = new List<LinkHandle>();
                foreach (Transform child in parents.Select(idea => idea.transform)) {
                    var idea = child.GetComponent<Idea>();
                    if (idea && !initialized.Contains(idea)) {
                        row.Add(CreateLink(idea, linkMatrix.Count));
                        initialized.Add(idea);
                    }
                }
                parents = linkMatrix.Last().Select(l => l.idea.subject).ToList();
                linkMatrix.Add(row);
            }
        }

        private LinkHandle CreateLink(Idea idea, int depth) {
            var prefab = linkPrefab.gameObject;
            var go = ObjectUtils.Instantiate(prefab, transform, runInEditMode: true);

            go.transform.position = idea.transform.position;
            var link = go.GetComponent<LinkHandle>();
            link.idea = idea;
            link.UpdatePosition();

            return link;
        }
    }

}

#endif