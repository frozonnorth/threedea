using UnityEngine;
using System.Collections;
using System;

public class Idea : MonoBehaviour {
    public Idea subject;
    public Transform anchor;

    internal void UpdatePosition() {
        transform.position = anchor.position;
    }
}
