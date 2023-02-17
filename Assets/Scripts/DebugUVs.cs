using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUVs : MonoBehaviour {
    [SerializeField] Vector2 position = Vector2.zero;
    [SerializeField] Vector2 size = Vector2.one;
    [SerializeField] float angle = 180f;

    void OnValidate() {
        //Debug.Log((Quaternion.AngleAxis(angle, Vector3.forward) * (position - size)) + size.ToV3());
    }
}
