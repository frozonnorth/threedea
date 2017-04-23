using UnityEngine;
using System.Collections;
using System;

namespace Threedea {
    namespace UI {
        public class LinkHoverMenu : MonoBehaviour {
            [SerializeField]
            private SelectionRing selectionRing;

            internal void Hide() {
                selectionRing.Hide();
            }

            internal void Dock() {
                selectionRing.Dock();
            }

        }
    }
}