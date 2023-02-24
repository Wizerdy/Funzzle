using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolsBoxEngine;

public abstract class ReferenceSetter<T> : MonoBehaviour where T : class{
    [SerializeField] T source;
    [SerializeField] Reference<T> target;

    private void Reset() {
        source = GetComponent<T>();
    }

    private void Awake() {
        (target as IReferenceSetter<T>).SetInstance(source);
    }

    private void OnDestroy() {
        (target as IReferenceSetter<T>).SetInstance(null);
    }
}
