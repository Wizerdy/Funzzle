using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableParent : MonoBehaviour, IDraggable {
    public GameObject GameObject => gameObject;

    public void Merge(DraggableParent other) {
        if (other == this) { throw new System.AccessViolationException("Same object " + name); }

        while (other.transform.childCount > 0) {
            other.transform.GetChild(0).parent = transform;
        }

        other.gameObject.SetActive(false);
        Destroy(other.gameObject);
    }

    public void OnPickup() {

    }

    public void OnDrop() {
        Transform[] childs = new Transform[transform.childCount];
        for (int i = transform.childCount - 1; i >= 0; --i) {
            childs[i] = transform.GetChild(i);
        }

        for (int i = 0; i < childs.Length; i++) {
            childs[i].GetComponent<IDraggable>()?.OnDrop();
        }
    }
}
